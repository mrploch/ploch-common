# #286 — Move MAUI types out of the `Ploch.Lists.UI.MauiUI.*` namespaces

**Type:** refactor (breaking)
**Packages:** `Ploch.Common.Maui`, `Ploch.Common.Maui.Fonts`

## Summary

Two namespaces left over from the original extraction of these types out of the
`ploch-lists` repository have been realigned with the packages that actually ship them:

| Before | After |
|---|---|
| `Ploch.Lists.UI.MauiUI.ViewModels` | `Ploch.Common.Maui.ViewModels` |
| `Ploch.Lists.UI.MauiUI.Common.Fonts` | `Ploch.Common.Maui.Fonts` |

The first held the public `IViewModel` interface, so a consumer of `Ploch.Common.Maui` had to
import an unrelated product's namespace alongside `Ploch.Common.Maui.Views` to derive from
`BaseContentPage` or `BaseContentView`. The second was declared across the six icon-font glyph
classes in `Ploch.Common.Maui.Fonts` plus `IFontsBuilder` in `Ploch.Common.Maui`.

Beyond the confusion, the stray namespace actively misled diagnosis: while investigating #197,
`CS0234: The type or namespace name 'Lists' does not exist in the namespace 'Ploch'` read like a
missing cross-repository dependency when the type had been in the referenced assembly all along.

`IViewModel` also gained the XML documentation it was missing.

## BREAKING CHANGE

`Ploch.Lists.UI.MauiUI.ViewModels.IViewModel` is now
`Ploch.Common.Maui.ViewModels.IViewModel`, and the types in
`Ploch.Lists.UI.MauiUI.Common.Fonts` (`FontAwesomeBrands`, `FontAwesomeRegular`,
`FontAwesomeSolid`, `MaterialDesignRegularFont`, `MaterialDesignWebFont`, `MauiMaterialAssets`,
`IFontsBuilder`) are now in `Ploch.Common.Maui.Fonts`. Consumers must update their `using`
directives; no type names, signatures, or behaviour changed.

**Impact in practice: none.** Neither `Ploch.Common.Maui` nor `Ploch.Common.Maui.Fonts` has ever
been published to nuget.org or GitHub Packages, and neither project is part of
`Ploch.Common.slnx` — they live only in `Ploch.Common.Endpoints.slnx`. There are no consumers to
break, which is precisely why the rename was done now rather than after first publication.

Refs: #286, #197
