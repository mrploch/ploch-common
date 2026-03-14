# Ploch.Common.Maui

> Base classes, view/view-model discovery, font management, and configuration helpers for .NET MAUI applications.

## Overview

`Ploch.Common.Maui` is split across two packages:

- **`Ploch.Common.Maui`** — The main library, providing MVVM base classes, automatic view and view-model discovery with DI registration, configuration helpers (JSON resource files, `Preferences`-backed configuration), XAML markup extensions, and the font management system.
- **`Ploch.Common.Maui.Fonts`** — Pre-built glyph constant classes for common icon fonts: Font Awesome (Solid, Regular, Brands), Material Design Icons, Material Design Web Font, and MAUI Material Assets.

Together these packages provide the boilerplate that every MAUI application needs, covering the lifecycle integration between views and view models, font setup, and `IConfiguration` wiring.

## Installation

```shell
# Main MAUI utilities
dotnet add package Ploch.Common.Maui

# Icon font glyph constants
dotnet add package Ploch.Common.Maui.Fonts
```

## Key Types

### Views and view models

| Type | Kind | Description |
|---|---|---|
| `IViewModel` | Interface | Defines `OnAppearingAsync()` — called when the associated view becomes visible. **Note:** currently in namespace `Ploch.Lists.UI.MauiUI.ViewModels` rather than `Ploch.Common.Maui.ViewModels` — this appears to be a leftover from the original project and may be moved in a future release. |
| `BaseViewModel` | Abstract class | Inherits `ObservableObject` (CommunityToolkit.Mvvm). Implements `IViewModel` with a no-op `OnAppearingAsync`. |
| `IView` | Interface | Marker interface for MAUI views participating in the discovery system. |
| `BaseContentPage` | Abstract class | Inherits `ContentPage`. Constructor accepts `IViewModel`. Sets `BindingContext` and calls `OnAppearingAsync` on each `OnAppearing` event. |
| `BaseContentView` | Abstract class | Inherits `ContentView`. Exposes a settable `ViewModel` property. Calls `OnAppearingAsync` when the view first renders and provides `OnViewDisappeared` for cleanup. |

### Discovery and DI registration

| Type | Kind | Description |
|---|---|---|
| `TypeDiscoverer` | Static class | `DiscoverViewModels(assemblies?)` — finds all non-abstract `IViewModel` implementations. `DiscoverViews(assemblies?)` — finds all non-abstract `IView` implementations. |
| `AppConfigurator` | Static class | `AddViewModels<TAssemblyMarker>()` and `AddViews<TAssemblyMarker>()` — register discovered types as singletons. |

### Configuration

| Type | Kind | Description |
|---|---|---|
| `JsonConfigurationResourceFileRegistrations` | Static class | `AddResourceJsonFileConfiguration<TBuilder>()` — adds `appsettings.json` and a platform-specific `appsettings.{Platform}.json` embedded as manifest resources. `AddOptions<TOptions>(sectionName?)` — wires an options class from a named section. |
| `PreferencesConfigurationSource` | Class | `IConfigurationSource` skeleton backed by MAUI `IPreferences`. **Warning:** `Build()` currently throws `NotImplementedException`. The associated `PreferencesConfigurationProvider` supports `TryGet`, `Set`, and `GetChildKeys`, but its `Load()` method also throws `NotImplementedException`. This class is not yet fully implemented. |

### Font management

| Type | Kind | Description |
|---|---|---|
| `FontMapping` | Record | Associates a font family name with its file name. Optionally holds a `FontConstantsType` for glyph lookup. `GetGlyphValue(glyphName)` and `GetAllGlypns()` use reflection with internal caching. |
| `IconFontMapping<TFontIconNames>` | Record | Specialisation of `FontMapping` for icon fonts; carries a glyph-constants type as a generic parameter. |
| `IFontsBuilder` | Interface | Fluent API for registering font mappings: `AddFont(fileName, family, setter)`. |
| `IFontsProvider` | Interface | Font lookup: `GetFonts()`, `GetFont(family)`, `TryGetFont(family, out mapping)`, `GetGlyph(family, glyphName)`. |
| `AppFonts` | Class | Singleton (`AppFonts.Instance`) that implements both `IFontsBuilder` and `IFontsProvider`. Pre-registers Material Icons, Material Design Web Icons, Font Awesome (Regular, Brands, Solid), Open Sans, MAUI Material Assets, and Inter. |

### XAML markup extensions

| Type | Kind | Description |
|---|---|---|
| `NameOfExtension` | Class | XAML markup extension providing `nameof`-equivalent behaviour in XAML: `{local:NameOf Type={x:Type MyClass} Member=MyProperty}` returns the property name as a string. |

### Icon font glyph constants (`Ploch.Common.Maui.Fonts`)

| Class | Font |
|---|---|
| `FontAwesomeSolid` | Font Awesome 6 Free Solid |
| `FontAwesomeRegular` | Font Awesome 6 Free Regular |
| `FontAwesomeBrands` | Font Awesome 6 Free Brands |
| `MaterialDesignRegularFont` | Material Icons Regular |
| `MaterialDesignWebFont` | Material Design Icons Web Font |
| `MauiMaterialAssets` | MAUI built-in material assets |

Each class contains `public const string` fields named after glyphs (e.g. `FontAwesomeSolid.Home`, `MaterialDesignRegularFont.Settings`).

## Usage Examples

### Registering views and view models in MauiProgram.cs

```csharp
var builder = MauiApp.CreateBuilder();

builder.Services
    .AddViewModels<App>()   // discovers all IViewModel implementations in the assembly
    .AddViews<App>();       // discovers all IView implementations in the assembly
```

### Implementing a page with a view model

```csharp
// ViewModel
public class HomeViewModel : BaseViewModel
{
    public string Greeting { get; set; } = "Hello, MAUI!";

    public override async Task OnAppearingAsync()
    {
        // load data when the page becomes visible
        await LoadDataAsync();
    }
}

// View
public partial class HomePage : BaseContentPage
{
    public HomePage(HomeViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
```

### Loading JSON configuration from embedded resources

Embed `appsettings.json` and `appsettings.Android.json` as `EmbeddedResource` in the MAUI project, then:

```csharp
builder.Configuration.AddResourceJsonFileConfiguration();

// Bind a section to a strongly typed options class
builder.AddOptions<ApiOptions>(); // reads the "ApiOptions" section
```

### Using the font system

```csharp
// Access pre-registered fonts via the singleton
var solidFont = AppFonts.Instance.FontAwesomeSolid;
string homeGlyph = solidFont.GetGlyphValue(nameof(FontAwesomeSolid.Home))!;

// Or via IFontsProvider (injectable)
public class IconService(IFontsProvider fonts)
{
    public string GetIcon(string glyphName)
        => fonts.GetGlyph("FontAwesomeSolid", glyphName) ?? string.Empty;
}
```

### Registering fonts with the MAUI builder

```csharp
builder.ConfigureFonts(fonts =>
{
    foreach (var (_, mapping) in AppFonts.Instance.Fonts)
    {
        fonts.AddFont(mapping.FontFileName, mapping.FontFamily);
    }
});
```

### Using NameOfExtension in XAML

```xml
<Label Text="{local:NameOf Type={x:Type vm:HomeViewModel}, Member=Greeting}" />
```

This produces the string `"Greeting"` at runtime, avoiding magic strings for property name bindings.

## Related Libraries

- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — ServicesBundle pattern that can complement MAUI DI setup
