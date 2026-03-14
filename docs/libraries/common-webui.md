# Ploch.Common.WebUI

> Shared view model helpers and tag utility extensions for ASP.NET Core MVC / Razor Pages applications.

## Overview

`Ploch.Common.WebUI` provides a small set of helpers that address recurring needs in ASP.NET Core MVC and Razor Pages projects: tracking the currently active page in a shared layout and building strongly-typed `SelectListItem` collections from domain model collections.

The `AppPage` record gives a common representation of a named route so that layouts and navigation partials can highlight the active menu item without relying on magic strings scattered across views. The `SelectListHelper` reduces the ceremony of converting a list of domain objects into the `IList<SelectListItem>` shape required by `<select>` tag helpers.

## Installation

```shell
dotnet add package Ploch.Common.WebUI
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `AppPage` | Record | Represents a navigable page with `Name`, `Path`, and `Title` properties. |
| `AppPageViewDataExtensions` | Static class | Extension methods on `ViewDataDictionary`: `SetCurrentPage`, `GetCurrentPage`, `GetPageTitle`. Uses the key `"CurrentPage"`. |
| `SelectListHelper` | Static class | `CreateFor<TModel>` — maps any collection to `IList<SelectListItem>` using delegate-provided text and value selectors. |

## Usage Examples

### Marking the active page in a controller action

```csharp
public IActionResult Index()
{
    ViewData.SetCurrentPage(new AppPage("Home", "/", "Home - My App"));
    return View();
}
```

### Reading the active page in a shared layout

```razor
@using Ploch.Common.WebUI
@{
    var page = ViewData.GetCurrentPage();
    var title = ViewData.GetPageTitle() ?? "My App";
}
<title>@title</title>
<nav>
    <a class="@(page?.Path == "/" ? "active" : "")" href="/">Home</a>
    <a class="@(page?.Path == "/blog" ? "active" : "")" href="/blog">Blog</a>
</nav>
```

### Building a select list from a domain collection

```csharp
// In a controller action:
var categories = await _categoryRepository.GetAllAsync();
ViewBag.Categories = SelectListHelper.CreateFor(
    categories,
    textFunc: c => c.Name,
    valueFunc: c => c.Id,
    includeNull: true,
    nullText: "-- Select category --");
```

```razor
@* In the view: *@
<select asp-items="ViewBag.Categories" name="CategoryId"></select>
```

`CreateFor` accepts an optional `includeNull` flag (default `false`). When `true`, a leading empty item is prepended with the text supplied by `nullText` (default `"--- Select ---"`).

## Related Libraries

- [Ploch.Common.WebApi](common-webapi.md) — CRUD endpoint infrastructure for API layers
- [Ploch.Common.Web](common-web.md) — Swagger configuration helpers
