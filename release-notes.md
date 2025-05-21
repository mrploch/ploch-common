# Release Notes

## 2.0.1

### Breaking Changes

- Removed `Ploch.Common.Data` libraries - those will be published separately.

### New Features

#### Ploch.Common.Serialization

Serialization abstraction library that provides a common interface for serialization and deserialization of objects.
Currently supported serializers are:

- [Newtonsoft Json.NET](https://www.newtonsoft.com/json) in
  the [Ploch.Common.Serialization.NewtonsoftJson](../Common.Serialization.NewtonsoftJson/README.md) package,
- [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json) in
  the [Ploch.Common.Serialization.SystemTextJson](../Common.Serialization.SystemTextJson/README.md) package

More available here: [Ploch.Common.Serialization](./src/Common.Serialization/README.md)

