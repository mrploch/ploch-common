# #204 — Reflection namespace article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the whole of `Ploch.Common.Reflection` to the Articles section of
the documentation site. It is organised around the four jobs the namespace actually does — asking
questions about a `Type`, discovering implementations across assemblies, reading and writing members
by name, and walking or comparing object graphs — rather than around the type list.

## Details

New page `DocumentationSite/articles/reflection.md`, wired into
`DocumentationSite/articles/toc.yml`.

It covers:

- **`TypeExtensions`** — `IsImplementing` including its open-generic support, and the two behaviours
  that differ from `IsAssignableFrom`: a type never reports as implementing *itself*, and
  `concreteOnly: true` (the default everywhere else in the namespace) excludes structs and static
  classes as well as abstract classes; `IsConcreteImplementation`; `IsSimpleType` and how much wider
  than "primitive" it is — every value type qualifies, so `DateTime`, `Guid`, any user struct and any
  `ValueTuple` are treated as atomic values downstream; `IsNullable`, `IsEnumerable`; and
  `GetReadableTypeName`, kept consistent with the merged expressions article, plus its three
  boundaries (CLR names not C# keywords, no namespace or declaring type, and multi-dimensional arrays
  losing their rank — `int[,]` renders as `Int32[]`).
- **Assembly scanning** — a comparison table of `ImplementationTypes`, `AssemblyTypes` and
  `TypeLoader` across scope, filters, execution model and, most importantly, `ReflectionTypeLoadException`
  behaviour. Only `AssemblyTypes` recovers partial results; the other two propagate. Demonstrated with
  a real assembly deployed without its dependency, showing the exact outcome of each of the four calls.
- **`TypeLoader` configuration** — multiple base types are OR not AND; abstract/interface/struct types
  are excluded by default; the globs match `Type.FullName` and the assembly's simple name and are
  `StringComparison.Ordinal`, so a case-mismatched pattern silently returns nothing; results are a set;
  no filters means every type including compiler-generated ones; and `LoadTypes(params Type[])` rejects
  an empty array.
- **Member access by name** — `PropertyHelpers` with its named `PropertyAccessException` family and
  their exact messages; the two leaks out of that family (`InvalidCastException` from the typed
  overload, `IndexOutOfRangeException` when an index is passed for a non-indexed property); the
  `SetPropertyValue`/`GetPropertyValue` asymmetry, where writes resolve against `typeof(T)` and so
  fail through a base-typed or `object`-typed variable while reads succeed; `GetPropertyValues`
  throwing on write-only properties; the static-member helpers, including that
  `TryGetStaticPropertyValue` is public-only and that `GetStaticFieldValues<TType>` cannot target a
  `static` class at all (`CS0718`); and `ObjectReflectionExtensions`/`MemberInfoExtensions`.
- **Object graphs, with the caveats stated rather than buried** — `ExecuteOnProperties` visits
  non-enumerable property values twice, walks strings character by character, and does not terminate
  on cyclic graphs because its `visited` set is populated but never consulted;
  `ByValueObjectComparator` compares collections by their `Count` and `Capacity` rather than their
  contents (so two lists with different items compare equal), which also makes
  `ByValueObjectComparer<T>` violate the `IEqualityComparer<T>` contract for collection-bearing types
  since `ObjectHashCodeBuilder` *does* hash elements, and it overflows the stack on cyclic graphs;
  `ObjectHashCodeBuilder` is the sturdy one — cycle-safe and resilient to throwing getters, but not
  stable across processes.
- A cost-and-caching section and a quick-reference table.

Every code sample was compiled against the real `Ploch.Common` API, and every behavioural claim —
return values, exception types and messages, visit order, glob case-sensitivity, deferred execution,
`ReflectionTypeLoadException` recovery, hash stability — was verified by executing it and quoting the
real output.

No changes to shipped library code — packages are unaffected.
