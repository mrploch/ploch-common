# EnumerableExtensions

This is a static class with extension methods related to `IEnumerable`.

> Docs entirely written by AI :)

## Extensions

### ValueIn

```csharp
public static bool ValueIn<TValue>(this TValue value, IEqualityComparer<TValue>? comparer, params TValue[] values)
```

This method checks if a set of `values` contain the provided `value` using a provided `comparer`.

Here is an example of use:

```csharp
bool result = "value".ValueIn(StringComparer.OrdinalIgnoreCase, "VALUE");
```

In this example, `result` will be `true` because "value" is in the list of provided values, using a case-insensitive
comparison.

### ValueIn

```csharp
public static bool ValueIn<TValue>(this TValue value, params TValue[] values)
```

Checks if a set of `values` contain the provided `value` using default comparer.

Example:

```csharp
bool result = "value".ValueIn("value", "anotherValue");
```

In this case, `result` will be `true` because "value" is in the list of provided values, using the default string
comparer.

### None

```csharp
public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
```

Verifies that none of the items in the `source` collection matches the provided `predicate`.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
bool result = source.None(x => x > 3);
```

In this case, `result` will be `true` because none of the values in the `source` collection are greater than 3.

### Join

```csharp
public static string Join<TValue>(this IEnumerable<TValue> source, string separator)
```

Joins the elements of the `source` collection using the provided `separator`, calling `ToString` on each element of the
collection.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
string result = source.Join(", ");
```

In this case, `result` will be `"1, 2, 3"`.

In all examples above, `TValue` and `TSource` are placeholder type names. Replace them with the actual types when using
the methods.

### Join

```csharp
public static string Join<TValue, TResult>(this IEnumerable<TValue> source, string separator, Func<TValue, TResult> valueSelector)
```

Joins the elements of the `source` collection using the provided `separator`, calling `valueSelector` on each element.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
string result = source.Join(", ", num => $"Item {num}");
```

In this case, `result` will be `"Item 1, Item 2, Item 3"`.

### JoinWithFinalSeparator

```csharp
public static string JoinWithFinalSeparator<TValue>(this IEnumerable<TValue> source, string separator, string finalSeparator)
```

Joins the elements of the `source` collection using the provided `separator`, calling `ToString` on each element of the
collection. The last element is joined using the `finalSeparator`.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
string result = source.JoinWithFinalSeparator(", ", " and ");
```

In this case, `result` will be `"1, 2 and 3"`.

### Shuffle

```csharp
public static IEnumerable<TValue> Shuffle<TValue>(this IEnumerable<TValue> source)
```

Randomly shuffles the elements of the `source` enumerable.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
IEnumerable<int> shuffled = source.Shuffle();
```

The `shuffled` variable will contain the elements `1`, `2`, `3` in a random order.

### TakeRandom

```csharp
public static IEnumerable<TValue> TakeRandom<TValue>(this IEnumerable<TValue> source, int count)
```

Takes random `count` amount of items from the `source` enumerable.

Example:

```csharp
IEnumerable<int> source = new[] {1, 2, 3};
IEnumerable<int> randomTwo = source.TakeRandom(2);
```

The `randomTwo` variable will contain two random elements from the source collection.

Please note that `TValue` is a placeholder type. Replace it with the actual type when using the methods.

### If

```csharp
public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, bool condition, Func<IEnumerable<T>, IEnumerable<T>> action)
```

This method is used to conditionally perform an action on the `enumerable`. If the `condition` is `true`, the `action`
is performed on the `enumerable`.

Example:

```csharp
IEnumerable<int> numbers = new[] {1, 2, 3, 4, 5};
var hasFilterCondition = true;
numbers = numbers.If(hasFilterCondition, nums => nums.Where(num => num > 3));
```

In this example, if `hasFilterCondition` is `true`, numbers greater than `3` are included in `numbers`.

### ForEach

```csharp
public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
```

Performs the specified `action` on each element of the `enumerable`.

Example:

```csharp
IEnumerable<int> numbers = new[] {1, 2, 3, 4, 5};
numbers.ForEach(num => Console.WriteLine(num));
```

In this example, the `ForEach` method is used to write each number in the console.

### AreSequentialInOrder

```csharp
public static bool AreSequentialInOrder<T>(this IEnumerable<T> enumerable) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
```

Determines whether the elements in the given `enumerable` are in sequential order.

Example:

```csharp
IEnumerable<int> numbers = new[] {1, 2, 3, 4, 5};
bool areSequential = numbers.AreSequentialInOrder();
```

In this example, `areSequential` will be `true` because the numbers are in sequential order.

### NullOrEmpty

```csharp
public static bool NullOrEmpty<T>(this IEnumerable<T>? enumerable)
```

Checks if the specified `enumerable` collection is `null` or empty.

Example:

```csharp
IEnumerable<int> numbers = null;
bool isNullOrEmpty = numbers.NullOrEmpty();  
```

In this example, `isNullOrEmpty` will be `true` because the `numbers` collection is `null`.

