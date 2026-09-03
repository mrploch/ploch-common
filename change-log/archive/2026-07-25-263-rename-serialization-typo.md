# Newtonsoft.Json serializer package renamed to fix a long-standing typo

**Issue:** [#263](https://github.com/mrploch/ploch-common/issues/263)

## Breaking Changes

- The Newtonsoft.Json serialization package, assembly, and namespace are renamed from the
  misspelled **`Ploch.Common.Serialiation.NewtonsoftJson`** ("Serialiation", missing the `z`)
  to the correct **`Ploch.Common.Serialization.NewtonsoftJson`**. The typo originated in the
  project file name and flowed into the package ID published on NuGet.org, the assembly name,
  and the public namespace of `NewtonsoftJsonObjectSerializer`.

  **Migration:**
  1. Replace the `PackageReference` to `Ploch.Common.Serialiation.NewtonsoftJson` with
     `Ploch.Common.Serialization.NewtonsoftJson`.
  2. Update `using Ploch.Common.Serialiation.NewtonsoftJson;` directives to
     `using Ploch.Common.Serialization.NewtonsoftJson;`.

  The old package ID remains on NuGet.org for existing consumers but receives no further
  updates and will be marked deprecated with a pointer to the new ID.
