# Ploch.Common.DataAnnotations

> Custom `System.ComponentModel.DataAnnotations` validation attributes for date and time model properties.

## Overview

`Ploch.Common.DataAnnotations` is a small focused library that extends the standard .NET data annotation system with validation attributes not available in the BCL. It targets `netstandard2.0` and `net6.0`.

The library currently provides one attribute: `RequiredNotDefaultDateAttribute`, which validates that a date or date-time property has been explicitly set and is not the language default value. This is distinct from the standard `[Required]` attribute, which only tests for `null` — a `DateTime` property with `default` value (`DateTime.MinValue`, i.e. `0001-01-01`) is non-null and would pass `[Required]`, but would fail `[RequiredNotDefaultDate]`.

## Installation

```bash
dotnet add package Ploch.Common.DataAnnotations
```

## Key Types

| Type | Description |
|------|-------------|
| `RequiredNotDefaultDateAttribute` | Validates that a `DateTime`, `DateTimeOffset`, or `DateOnly` (net6.0+) property is not `null` and not equal to its default value. Returns invalid for any other type, including `TimeOnly`. |

## Usage Examples

### Model validation

```csharp
using System.ComponentModel.DataAnnotations;
using Ploch.Common.DataAnnotations;

public class CreateEventRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    // Ensures the date was explicitly provided and is not 0001-01-01
    [RequiredNotDefaultDate]
    public DateTime StartDate { get; set; }

    // Works with DateTimeOffset
    [RequiredNotDefaultDate]
    public DateTimeOffset PublishedAt { get; set; }

    // Works with DateOnly on .NET 6+
    [RequiredNotDefaultDate]
    public DateOnly DueDate { get; set; }
}
```

### Manual validation

```csharp
var attribute = new RequiredNotDefaultDateAttribute();

attribute.IsValid(DateTime.Now);         // true
attribute.IsValid(default(DateTime));    // false  (0001-01-01 00:00:00)
attribute.IsValid(null);                 // false
attribute.IsValid(DateTimeOffset.Now);   // true
attribute.IsValid(default(DateTimeOffset)); // false
```

### Default error message

The default message uses the standard `{0}` placeholder for the display name of the property:

```
The StartDate field is required and must not be the default date or date time value.
```

You can override the message through the standard `ErrorMessage` property:

```csharp
[RequiredNotDefaultDate(ErrorMessage = "Please provide a valid start date.")]
public DateTime StartDate { get; set; }
```

## Supported Types

| Type | Supported | Notes |
|------|-----------|-------|
| `DateTime` | Yes | All target frameworks |
| `DateTimeOffset` | Yes | All target frameworks |
| `DateOnly` | Yes | net6.0 and later only |
| `TimeOnly` | No | Returns invalid for `TimeOnly` values |
| Other types | No | Returns invalid |

## Related Libraries

- [Ploch.Common](common.md) — Core library with `Guard` argument checking that complements model-level validation with code-level precondition enforcement.
