# Ploch.Common.ObjectBuilder

> Utilities for constructing strongly-typed .NET objects from loosely-typed property bags, with built-in support for WMI management object sources.

## Overview

`Ploch.Common.ObjectBuilder` provides infrastructure for mapping arbitrary key-value property sources to typed .NET objects. The primary use case is consuming Windows Management Instrumentation (WMI) objects — instances of `System.Management.ManagementObject` — and projecting them onto well-typed POCOs.

The library follows a converter pipeline pattern. An `ISourceObject` abstraction represents the property source. `ObjectConverter` iterates the source's property names, matches them to target object properties (using optional `ObjectPropertyAttribute` renaming), and passes each raw value through an ordered chain of `ITypeConverter` instances until one handles the conversion.

Built-in converters handle WMI-encoded date strings (converting them to `DateTime` and `DateTimeOffset`), string-to-enum mapping (via the `EnumConverter` from `Ploch.Common.TypeConversion`), and fall-through conversion via `System.Convert.ChangeType`.

The library requires `System.Management` and therefore targets Windows only (the `System.Management` package is Windows-specific). It also depends on `Ploch.Common`.

## Installation

```bash
dotnet add package Ploch.Common.ObjectBuilder
```

## Key Types

| Type | Description |
|------|-------------|
| `ISourceObject` | Abstraction over a loosely-typed property bag: `GetPropertyNames()`, `GetPropertyValue(string)`, `GetProperties()`. Implement this to wrap any property source. |
| `ITypeConverter` | Converter interface with `Order`, `CanHandle(value, targetType)`, and `ConvertValue(value, targetType)`. Lower `Order` values are tried first. |
| `ObjectConverter` | Static class with `BuildObject<TManagementObject>(ISourceObject)`. Orchestrates the converter pipeline to populate a new `TManagementObject` instance. |
| `ObjectPropertyAttribute` | Apply to target POCO properties to remap a differently-named source property. |
| `ManagementObjectDateTimeOffsetTypeConverter` | Converts WMI date strings to `DateTimeOffset`. **Note:** This converter is not yet implemented; it currently throws `NotImplementedException`. |
| `ManagementObjectDateTimeTypeConverter` | Converts WMI date strings to `DateTime` (UTC). |
| `DotNetTypeConverterWrapper` | Wraps a `System.ComponentModel.TypeConverter` as an `ITypeConverter`. |
| `DefaultConverter` | Fallback converter (`CanHandle` always returns `true`). **Note:** `ConvertValue` currently throws `NotImplementedException`; only the `CanHandle(Type, Type)` overload is functional, which checks registered converters and `TypeDescriptor`. |

## Usage Examples

### Implementing ISourceObject for a WMI ManagementObject

```csharp
using System.Management;
using Ploch.Common.ObjectBuilder;

public class ManagementObjectSource : ISourceObject
{
    private readonly ManagementObject _obj;

    public ManagementObjectSource(ManagementObject obj) => _obj = obj;

    public IEnumerable<string> GetPropertyNames() =>
        _obj.Properties.Cast<PropertyData>().Select(p => p.Name);

    public object GetPropertyValue(string propertyName) =>
        _obj[propertyName];

    public IDictionary<string, object> GetProperties() =>
        _obj.Properties.Cast<PropertyData>()
            .ToDictionary(p => p.Name, p => p.Value);
}
```

### Defining the target POCO

```csharp
using Ploch.Common.TypeConversion;

public class DiskDrive
{
    public string? Caption { get; set; }

    public string? DeviceID { get; set; }

    // WMI property name is "Size", mapped directly by convention
    public ulong Size { get; set; }

    // Remap a WMI property with a different name
    [ObjectProperty("InstallDate")]
    public DateTime? InstalledOn { get; set; }

    public DiskDriveStatus? Status { get; set; }
}

public enum DiskDriveStatus { OK, Error, Degraded, Unknown }
```

### Building the object

```csharp
using System.Management;
using Ploch.Common.ObjectBuilder;

using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
foreach (ManagementObject mo in searcher.Get())
{
    var source = new ManagementObjectSource(mo);
    var drive = ObjectConverter.BuildObject<DiskDrive>(source);
    Console.WriteLine($"{drive.Caption} — {drive.Size / (1024 * 1024 * 1024)} GB");
}
```

### Property renaming with ObjectPropertyAttribute

When the source property name differs from the target property name, decorate the target property:

```csharp
[ObjectProperty("Win32_PropertyName")]
public string MyProperty { get; set; } = string.Empty;
```

`ObjectConverter` builds an internal map from source names to target properties at call time, applying the attribute where present.

## Converter Pipeline

`ObjectConverter` maintains a static ordered list of `ITypeConverter` instances:

1. `ManagementObjectDateTimeOffsetTypeConverter` (converts WMI DMTF date strings to `DateTimeOffset`)
2. `ManagementObjectDateTimeTypeConverter` (converts WMI DMTF date strings to `DateTime` UTC)
3. `EnumConverter` (converts string values to nullable enum types by field name)
4. `DefaultConverter` (falls back to `System.Convert.ChangeType` and registered `TypeConverter`s)

Converters are tried in `Order` sequence. The first converter where `CanHandle` returns `true` performs the conversion. If no converter handles the value, the implementation falls back to `null` (for null input) or `Convert.ChangeType`; conversion failures may then throw.

## Extending the Pipeline

The current `ObjectConverter` uses a fixed static pipeline. To add custom converters, implement `ITypeConverter` (from `Ploch.Common.TypeConversion`) and contribute to the converter list by sub-classing or modifying `ObjectConverter`. Custom converters should set `Order` to a value lower than `int.MaxValue` to take precedence over the default fall-through converter.

```csharp
public class GuidTypeConverter : ITypeConverter
{
    public int Order => 10;

    public bool CanHandle(object? value, Type targetType) =>
        value is string && targetType == typeof(Guid);

    public bool CanHandleSourceType(Type sourceType) => sourceType == typeof(string);

    public bool CanHandleTargetType(Type targetType) => targetType == typeof(Guid);

    public bool CanHandle(Type sourceType, Type targetType) =>
        CanHandleSourceType(sourceType) && CanHandleTargetType(targetType);

    public object? ConvertValue(object? value, Type targetType) =>
        value is string s ? Guid.Parse(s) : null;
}
```

## Related Libraries

- [Ploch.Common](common.md) — Provides `TypeConversion.EnumConverter`, `TypeConversion.SingleSourceTargetTypeConverter`, `TypeConversion.TypeConversionException`, and `ArgumentChecking.Guard` used throughout this library.
