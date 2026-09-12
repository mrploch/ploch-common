# Expressions and Owned Properties

The `Ploch.Common.Linq` namespace contains three closely related pieces:

| Type | Role |
|---|---|
| `ExpressionExtensions` | Extracts member names, and property handles, from lambda expressions. |
| `IOwnedPropertyInfo` (and its generic forms) | A `PropertyInfo` bound to the instance that owns it. |
| `OwnedPropertyInfo` | The concrete implementations. |

```csharp
using Ploch.Common.Linq;
```

## The problem these solve

Reflection code habitually splits a property into two halves that have to be kept in step: a
`PropertyInfo` describing *which* property, and an object reference describing *whose*. Every call
site then repeats the pairing:

```csharp
var propertyInfo = typeof(Customer).GetProperty(nameof(Customer.EmailAddress));
propertyInfo.SetValue(customer, newAddress);      // instance passed again, every time
var current = (string?)propertyInfo.GetValue(customer);
```

`IOwnedPropertyInfo` binds the two together once, and `ExpressionExtensions.GetProperty` produces one
from a strongly typed lambda — so the property is chosen by the compiler rather than by a string.

---

## `GetMemberName` — turn a lambda into a member name

```csharp
public static string GetMemberName(this Expression<Action> expression)
public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression)
public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> expression)
```

All three overloads accept a member access **or a method call**, returning the member or method name.
The two-type-parameter overload additionally unwraps a `UnaryExpression` — which the compiler inserts
whenever the selected member needs an implicit conversion to match `TMember` (for example selecting an
`int` property through an `Expression<Func<Order, object>>`).

`nameof` covers the simple case, so `GetMemberName` earns its place where the member is chosen
*generically*, by a caller that does not know the concrete type at the point of use:

```csharp
public sealed class SortSpecification<TEntity>
{
    private readonly List<string> _sortFields = [];

    public SortSpecification<TEntity> ThenBy<TMember>(Expression<Func<TEntity, TMember>> selector)
    {
        _sortFields.Add(selector.GetMemberName());

        return this;
    }

    public string ToOrderByClause() => string.Join(", ", _sortFields);
}

var sort = new SortSpecification<Order>()
           .ThenBy(order => order.PlacedOn)
           .ThenBy(order => order.Total)        // int/decimal — unary conversion handled
           .ToOrderByClause();
// "PlacedOn, Total"
```

Because the name is derived from a compiled expression tree, renaming `Order.PlacedOn` in an IDE
updates the lambda automatically — an advantage a `"PlacedOn"` string literal cannot offer.

The parameterless `Expression<Func<TMember>>` overload works over a captured variable, which is handy
for logging the *name* of a local or field alongside its value:

```csharp
var retryCount = 3;
Expression<Func<int>> setting = () => retryCount;

_logger.LogDebug("{Setting} = {Value}", setting.GetMemberName(), retryCount);
// "retryCount = 3"
```

Note that a lambda has no type of its own, so it cannot be the receiver of an extension method
directly — assign it to an `Expression<...>` variable (or cast it) first, as above.

Passing an expression that is neither a member access, a method call, nor a unary wrapper around a
member access throws `InvalidOperationException`.

---

## `GetProperty` — bind a property to its owner

```csharp
public static IOwnedPropertyInfo<TType, TMember> GetProperty<TType, TMember>(this TType obj, Expression<Func<TType, TMember>> propertySelector)
```

`GetProperty` is an extension on the *instance*, so the owner is captured at the point the property is
selected:

```csharp
var customer = await _repository.GetAsync(customerId, cancellationToken);

var emailProperty = customer.GetProperty(c => c.EmailAddress);

Console.WriteLine(emailProperty.Name);       // "EmailAddress"
Console.WriteLine(emailProperty.GetValue()); // the current value — no instance argument
emailProperty.SetValue("new.address@example.com");
```

Passing anything other than a property selector — a field, a method call that is not a property
getter, a computed expression — throws `InvalidOperationException`.

---

## The `IOwnedPropertyInfo` hierarchy

Three interfaces build on each other, letting a consumer ask for exactly as much type information as
it can make use of:

```csharp
public interface IOwnedPropertyInfo
{
    string Name { get; }
    PropertyInfo PropertyInfo { get; }
    object Owner { get; }

    object? GetValue();
    object? GetValue(object[] index);
    void SetValue(object? value);
    void SetValue(object? value, object[] index);
}

public interface IOwnedPropertyInfo<TProperty> : IOwnedPropertyInfo
{
    new TProperty? GetValue();
    new TProperty? GetValue(object[] index);
    void SetValue(TProperty? value);
    void SetValue(TProperty? value, object[] index);
}

public interface IOwnedPropertyInfo<out TType, TProperty> : IOwnedPropertyInfo<TProperty>
{
    new TType Owner { get; }
}
```

- The non-generic form is what a general-purpose engine holds in a `List<IOwnedPropertyInfo>`.
- The single-parameter form gives typed reads and writes of the property value.
- The two-parameter form additionally exposes a typed `Owner`; `TType` is covariant, so an
  `IOwnedPropertyInfo<Customer, string>` can be used where `IOwnedPropertyInfo<IPerson, string>` is
  expected.

The matching classes — `OwnedPropertyInfo` (abstract), `OwnedPropertyInfo<TProperty>` and
`OwnedPropertyInfo<TType, TProperty>` — can be constructed directly from a `PropertyInfo` and an owner
when the property is discovered by reflection rather than selected by a lambda:

```csharp
var handles = customer.GetType()
                      .GetProperties()
                      .Where(property => property.PropertyType == typeof(string) && property.CanWrite)
                      .Select(property => new OwnedPropertyInfo<string>(property, customer))
                      .ToList();
```

Both the abstract base and the derived classes validate their constructor arguments, so a `null`
`PropertyInfo` or owner throws `ArgumentNullException` at construction rather than on first use.

---

## Worked example: a field-level change tracker

The pairing of a lambda-selected property with its owner makes a compact, type-safe audit helper:

```csharp
public sealed class ChangeTracker<TEntity>
{
    private readonly List<(IOwnedPropertyInfo Property, object? OriginalValue)> _tracked = [];

    public ChangeTracker<TEntity> Track<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> selector)
    {
        var property = entity.GetProperty(selector);
        _tracked.Add((property, property.GetValue()));

        return this;
    }

    public IEnumerable<FieldChange> GetChanges() =>
        _tracked.Where(entry => !Equals(entry.Property.GetValue(), entry.OriginalValue))
                .Select(entry => new FieldChange(entry.Property.Name, entry.OriginalValue, entry.Property.GetValue()));
}

public sealed record FieldChange(string FieldName, object? From, object? To);
```

Usage:

```csharp
var tracker = new ChangeTracker<Customer>()
              .Track(customer, c => c.EmailAddress)
              .Track(customer, c => c.PostCode)
              .Track(customer, c => c.MarketingOptIn);

ApplyUpdateRequest(customer, request);

foreach (var change in tracker.GetChanges())
{
    _auditLog.Record(customer.Id, change.FieldName, change.From, change.To);
}
```

Nothing here is stringly typed: renaming `Customer.PostCode` updates the lambdas, and the audit log
keeps producing the correct field names.

---

## Indexer properties

The `object[] index` overloads forward to `PropertyInfo.GetValue`/`SetValue` with index arguments, so
indexer properties are addressable too. The `"Item"` name that C# gives an indexer is available as
`Ploch.Common.Reflection.PropertyHelpers.IndexerPropertyName`:

```csharp
var indexer = new OwnedPropertyInfo<string>(
    settings.GetType().GetProperty(PropertyHelpers.IndexerPropertyName)!,
    settings);

var value = indexer.GetValue([ "ConnectionStrings:Default" ]);
```

---

## When to prefer something else

- **A single, known property on a known type** — use the property directly. This namespace exists for
  code that must work over properties it does not know at compile time.
- **A hot loop over thousands of instances** — reflection-based `GetValue`/`SetValue` is materially
  slower than a compiled delegate. Compile the expression to a `Func<TType, TMember>` once and reuse
  it instead.
- **Discovering which properties exist at all** — see
  [Reflection utilities](reflection.md), whose `PropertyHelpers` and `ObjectReflectionExtensions`
  handle enumeration, static properties and binding-flag control.

## See also

- [Reflection utilities](reflection.md)
- [`Ploch.Common.Linq` API reference](../api/Ploch.Common.Linq.html)
