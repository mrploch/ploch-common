# Expressions and Owned Property Info

`Ploch.Common.Linq` solves one narrow problem well: turning a strongly-typed lambda such as
`customer => customer.EmailAddress` into something a library can work with at runtime — a property
name, or a property paired with the object that owns it.

That matters whenever an API needs to *talk about* a member rather than simply read it. Sort-field
whitelists, audit trails, validation messages, mapping configuration and diagnostic logging all
need to name a member, and the naive implementation names it with a string literal. String literals
do not survive a rename, do not appear in "find usages", and fail at runtime rather than at compile
time. A lambda gets all three right.

The namespace contains two things:

| Type | Purpose |
|------|---------|
| <xref:Ploch.Common.Linq.ExpressionExtensions> | `GetMemberName` — the name of the member or method a lambda refers to. `GetProperty` — a property selector turned into an owned property. |
| <xref:Ploch.Common.Linq.IOwnedPropertyInfo> / <xref:Ploch.Common.Linq.OwnedPropertyInfo> | A `PropertyInfo` bound to a specific instance, so `GetValue()` and `SetValue(…)` need no `obj` argument. |

For a wider tour of the package, see the [Ploch.Common library guide](../../docs/libraries/common.md).

## When *not* to reach for this

Use `nameof` when the member is known at the call site:

```csharp
// Right: the compiler already knows the name.
logger.LogWarning("{Property} was empty", nameof(Customer.EmailAddress));

// Wrong: an expression tree allocated and walked to compute a compile-time constant.
Expression<Func<Customer, string?>> selector = c => c.EmailAddress;
logger.LogWarning("{Property} was empty", selector.GetMemberName());
```

`GetMemberName` earns its keep on the *other* side of that boundary — inside a generic method or a
fluent builder, where the member is supplied by the caller and the library itself cannot write a
`nameof`. Every example below is of that shape.

## Naming members with `GetMemberName`

There are three overloads, and picking the wrong one is the most common mistake:

```csharp
public static string GetMemberName(this Expression<Action> expression);
public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression);
public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> expression);
```

The third — the two-type-parameter one — is the one to reach for in library code, because it takes
a *parameterised* selector (`c => c.Name`) rather than a closure over an existing instance.

### A sortable-field whitelist

A search endpoint that accepts `?sort=name` must not pass the caller's string straight into a LINQ
`OrderBy`, or worse into SQL. The permitted set has to be declared somewhere, and declaring it with
lambdas keeps it honest under rename:

```csharp
public sealed class SortableFields<TEntity>
{
    private readonly HashSet<string> _allowed = new(StringComparer.OrdinalIgnoreCase);

    public SortableFields<TEntity> Allow<TMember>(Expression<Func<TEntity, TMember>> field)
    {
        _allowed.Add(field.GetMemberName());
        return this;
    }

    public bool IsAllowed(string requestedField) => _allowed.Contains(requestedField);
}
```

```csharp
var sortable = new SortableFields<Customer>()
               .Allow(c => c.Name)
               .Allow(c => c.RegisteredOn)
               .Allow(c => c.Id);

sortable.IsAllowed("name");         // true  — the comparer above makes matching case-insensitive
sortable.IsAllowed("emailaddress"); // false — never declared, so never sortable
```

`TMember` is inferred separately for each call, so `string`, `DateTimeOffset` and `int` members
coexist in one fluent chain without the caller casting anything.

### Boxing casts are unwrapped — on one overload only

Declaring the selector as `Expression<Func<TEntity, object>>` is a common way to hold heterogeneous
selectors in a single collection. Doing so boxes a value-type member, and the compiler wraps the
member access in a `UnaryExpression`. The two-type-parameter overload unwraps it:

```csharp
Expression<Func<Customer, object>> boxed = c => c.Id;

boxed.GetMemberName();   // "Id"
```

The single-type-parameter overload does **not**:

```csharp
Expression<Func<object>> boxedNoType = () => customer.Id;

boxedNoType.GetMemberName();   // throws InvalidOperationException: "Not a member expression!"
```

If a selector might be boxed, use the `Expression<Func<TType, TMember>>` form.

### Method names, and the expression that is never invoked

The `Expression<Action>` and `Expression<Func<TMember>>` overloads both accept a method call and
return the method's name. The lambda is inspected, never compiled and never executed:

```csharp
Expression<Action> operation = () => customer.ToString();

operation.GetMemberName();   // "ToString" — ToString() is not called
```

That makes the pattern safe for naming an operation whose execution would be expensive or has side
effects: the naive alternative — invoking the method so that something inside it can report its own
name — actually runs it. Where the target is a method *group* rather than a call, a delegate is
cheaper and equally side-effect-free: `((Func<string?>)customer.ToString).Method.Name`. The expression
overload earns its keep when the caller writes a call with arguments, because a method group cannot
express one.

### Local variable names

The `Expression<Func<TMember>>` overload returns the name of a captured local, because the compiler
lifts locals into closure fields and the lambda body is a `MemberExpression` over that field:

```csharp
var connectionString = configuration["Db:Default"];
Expression<Func<string?>> local = () => connectionString;

local.GetMemberName();   // "connectionString"
```

This was the standard pre-C# 10 trick for guard clauses that report the caller's variable name. On
.NET 7 and later, prefer `CallerArgumentExpressionAttribute` — which is what
[`Ploch.Common.ArgumentChecking`](../../docs/libraries/common.md) uses — and keep this overload for
cases where the name must be captured and stored rather than reported immediately.

## Binding a property to its owner with `IOwnedPropertyInfo`

`PropertyInfo` on its own is only half of what most runtime property work needs; every call site
also has to carry the instance and remember to pass it:

```csharp
// Two things to keep in step, and nothing stops you passing the wrong instance.
var propertyInfo = typeof(Customer).GetProperty("EmailAddress");
propertyInfo.SetValue(customer, ((string?)propertyInfo.GetValue(customer))?.Trim());
```

`GetProperty` returns the pair as a single value:

```csharp
public static IOwnedPropertyInfo<TType, TMember> GetProperty<TType, TMember>(
    this TType obj,
    Expression<Func<TType, TMember>> propertySelector);
```

```csharp
var email = customer.GetProperty(c => c.EmailAddress);

email.SetValue(email.GetValue()?.Trim().ToLowerInvariant());

email.Name;                        // "EmailAddress"
email.Owner.Id;                    // 42 — Owner is typed as Customer, not object
email.PropertyInfo.PropertyType;   // System.String — the raw PropertyInfo is still available
```

`GetValue()` and `SetValue(…)` are typed as `TMember?` on the generic interface, so the
`ToLowerInvariant()` above needs no cast. Where the property type is irrelevant — a heterogeneous
list, say — assign to the non-generic <xref:Ploch.Common.Linq.IOwnedPropertyInfo>, whose members are
`object?`-based:

```csharp
IOwnedPropertyInfo untyped = customer.GetProperty(c => c.Id);

untyped.GetValue();   // 42, boxed as object
```

### The selector must read a property *of the parameter*

`GetProperty` pairs whichever `PropertyInfo` the selector's body resolves to with the `obj` it was
called on. It does not check that the two belong together, so a selector that reaches past the
lambda parameter is accepted at construction and only misbehaves on access.

A nested selector picks up the *inner* property and the *outer* owner:

```csharp
var city = customer.GetProperty(c => c.Address.City);

city.Name;                     // "City"
city.PropertyInfo.DeclaringType;   // Address
city.Owner;                    // the Customer — not the Address

city.GetValue();   // throws TargetException:
                   // "Object type Address does not match target type Customer."
```

A selector that closes over a *different* instance is worse, because it fails silently:

```csharp
var other = new Customer { Id = 7 };

customer.GetProperty(c => other.Id).GetValue();   // 42 — customer.Id, not other.Id
```

Both are compile-clean, so neither is caught for you. Keep selectors to a single member access on
the lambda parameter — `c => c.City` on the `Address` itself, rather than `c => c.Address.City` on
the `Customer`.

### Building an audit trail

Binding the owner in is what keeps a change-tracking helper short: capture the properties once,
snapshot their values, and compare later, without threading the instance through the API at all.

The snapshot is keyed by the owned property itself rather than by its `Name`, because a name is not
unique across the set: two owners of the same type both contribute a `Name`, and every indexer is
called `Item` (see [Indexers](#indexers) below). Keying by name would throw a duplicate-key
`ArgumentException` from `ToDictionary` in either case.

```csharp
public sealed class ChangeSnapshot
{
    private readonly IReadOnlyList<IOwnedPropertyInfo> _properties;
    private readonly Dictionary<IOwnedPropertyInfo, object?> _original;

    public ChangeSnapshot(params IOwnedPropertyInfo[] properties)
    {
        _properties = properties;
        _original = properties.ToDictionary(property => property, property => property.GetValue());
    }

    public IEnumerable<string> GetChanges()
    {
        foreach (var property in _properties)
        {
            var current = property.GetValue();
            if (!Equals(current, _original[property]))
            {
                yield return $"{property.Name}: '{_original[property]}' -> '{current}'";
            }
        }
    }
}
```

```csharp
var snapshot = new ChangeSnapshot(
    customer.GetProperty(c => c.Name),
    customer.GetProperty(c => c.EmailAddress));

customer.Name = "Ada King";

foreach (var change in snapshot.GetChanges())
{
    Console.WriteLine(change);
}

// Name: 'Ada Lovelace' -> 'Ada King'
```

`GetChanges` is a `yield`-based iterator, so the current values are read when the sequence is
enumerated, not when the method is called. That is the behaviour you want here — enumerate late and
you see the state at enumeration time — but it is a trap if the sequence is enumerated after the
entity has been reset or reloaded. Call `ToList()` at the point where the comparison is meant to be
frozen.

### Validation messages that cannot drift

The classic failure mode of a validation message is the property name in the text drifting away
from the property actually checked. Reading both from the same owned property makes that
impossible:

```csharp
public static class Require
{
    public static void NotBlank<TTarget>(TTarget target, Expression<Func<TTarget, string?>> selector)
    {
        var property = target.GetProperty(selector);

        if (string.IsNullOrWhiteSpace(property.GetValue()))
        {
            throw new ArgumentException($"{typeof(TTarget).Name}.{property.Name} must not be blank.", property.Name);
        }
    }
}
```

```csharp
Require.NotBlank(customer, c => c.Name);
```

The same shape works for a `ValidationResult` that needs a member name, or for a problem-details
response whose `errors` dictionary is keyed by property name.

## Indexers

`GetProperty` accepts an indexer selector. The body of `b => b[0]` is not a `MemberExpression` but a
call to the compiler-generated `get_Item` accessor; `GetProperty` recognises that special case and
resolves the underlying `Item` property:

```csharp
var slot = basket.GetProperty(b => b[0]);

slot.Name;   // "Item"
```

There is a genuine surprise here: **the index inside the selector is discarded.** It identifies the
indexer, not an element. Reading or writing requires supplying the index again, to the index-taking
overloads:

```csharp
slot.GetValue(new object[] { 0 });               // "first"
slot.SetValue("replacement", new object[] { 0 });
basket[0];                                       // "replacement"

slot.GetValue();   // throws TargetParameterCountException — no index supplied
```

Treat an owned indexer property as a handle on the indexer itself, reusable across indices, rather
than as a handle on one element.

## Failure modes

Each of these throws from `Ploch.Common.Linq` rather than producing a `NullReferenceException` or a
silently wrong answer, and the messages are worth recognising:

| Expression | Result |
|------------|--------|
| `customer.GetProperty(c => c.Id + 1)` | `InvalidOperationException`: `Provided c => (c.Id + 1) is not a property expression.` |
| `target.GetProperty(t => t.SomeField)` | `InvalidOperationException`: `Provided t => t.SomeField is not a property expression.` — the member resolves to a `FieldInfo`, and only properties are supported. |
| `((Expression<Func<object>>)(() => customer.Id)).GetMemberName()` | `InvalidOperationException`: `Not a member expression!` — use the two-type-parameter overload. |
| A `null` expression | `ArgumentNullException`, thrown by the guard clause before the tree is walked. |

Two more get past construction and fail — or lie — only on access; see
[the selector must read a property of the parameter](#the-selector-must-read-a-property-of-the-parameter):

| Expression | Result |
|------------|--------|
| `customer.GetProperty(c => c.Address.City)` | Constructs. `GetValue()` throws `TargetException`: `Object type Address does not match target type Customer.` |
| `customer.GetProperty(c => other.Id)` | Constructs. `GetValue()` silently reads `customer.Id`, ignoring `other`. |

Assigning an incompatible value through the non-generic interface fails at the reflection layer
rather than inside `Ploch.Common`:

```csharp
IOwnedPropertyInfo untyped = customer.GetProperty(c => c.Id);

untyped.SetValue("not an int");   // ArgumentException from PropertyInfo.SetValue
```

The generic `SetValue(TMember?)` overload rules that out at compile time, so prefer the typed
interface unless the value genuinely is not known until runtime.

## Cost and caching

`GetMemberName` and `GetProperty` walk an expression tree on every call, a tree is allocated at each
call site that builds one, and property access through `PropertyInfo` is reflection rather than a
direct call. That is entirely acceptable for configuration built once at start-up — the whitelist
and validation examples above — and it is the wrong tool inside a tight loop over a large result
set.

The usual remedy applies: do the expression work once and cache the outcome. `SortableFields<T>` is
already an example — the lambdas are walked while the whitelist is being built, and every subsequent
request touches only a `HashSet<string>`.

## See also

- <xref:Ploch.Common.Linq> — the full API reference for the namespace.
- [Ploch.Common library guide](../../docs/libraries/common.md) — the other namespaces in the package.
- [Collections samples](./collections-samples.md) — extension methods for `IEnumerable<T>` and friends.
