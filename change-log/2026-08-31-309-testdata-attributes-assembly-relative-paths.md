## `Ploch.TestingSupport` / `Ploch.TestingSupport.XUnit3`: test data files resolve against the test assembly

**Fixed data attributes resolving a relative file path against the process working directory.**
`TextFileDataAttribute` (and so `TextFileLinesDataAttribute`) and `JsonFileDataAttribute` in both
packages passed the supplied path through `Path.GetFullPath`, which anchors a relative path to
`Environment.CurrentDirectory`. Test data files handed to these attributes are almost always project
content copied next to the assembly by `CopyToOutputDirectory`, so the correct anchor is
`AppContext.BaseDirectory` — the directory the assembly was loaded from, which does not depend on how
the test host was launched.

A relative path is now combined with `AppContext.BaseDirectory` before being normalised; a rooted
path is used unchanged, so callers already passing an absolute path are unaffected.

| Attribute argument | Resolved (before) | Resolved (after) |
|---|---|---|
| `"TestData/cases.txt"` | `<working directory>/TestData/cases.txt` | `<assembly directory>/TestData/cases.txt` |
| `"C:\data\cases.txt"` | `C:\data\cases.txt` | `C:\data\cases.txt` |

The fault was latent rather than constant because the xUnit v3 runners in common use normalise the
working directory to the assembly directory before running tests. A host that does not — an IDE
runner, a custom Microsoft.Testing.Platform host, or any use of the attribute outside a normalising
runner — saw `ArgumentException: Could not find file at path: ...` naming an unrelated directory.

**Fixed `Ploch.TestingSupport.XUnit3.TestData.TextFileDataAttribute` reading a different path from
the one it validated.** The existence check ran against the resolved `path` while the read used the
raw `filePath` argument. The two happened to resolve identically before this change, so the defect
was invisible; once the anchor moved to the assembly directory they diverged and the read failed
after the check had passed. `Ploch.TestingSupport.XUnit3.TestData.JsonFileDataAttribute` had the same
divergence and is fixed with it.

**`Ploch.TestingSupport.TestData.JsonFileDataAttribute` now checks that the file exists** before
reading it, throwing `ArgumentException` naming the resolved path, which is what the other three
attributes already did. It previously let `File.ReadAllText` throw `FileNotFoundException`.

### Compatibility

This is consumer-visible. A test suite whose data file sits relative to the *working directory*
rather than the test assembly's output directory — and which relied on the old resolution — will now
fail to find it. Fix such a case by making the file project content with
`<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`, or by passing an absolute path,
which is honoured unchanged. No public API signature changed.

Refs: #309, #299
