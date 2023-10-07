# Ploch.Common.Data.Model

## Overview

Library provides a set of interfaces and classes that can be used to standardize the entity model.

## Motivation

In many database-driven apps, we need similar or even the same model types.

I often had to create entities like `Category` or a `Tag`.

Also, I find myself using the same properties on many entity types.

Things like `Name`, `Title`, `Description` and so on, not mentioning the `Id` property.

Standardization of those types brings a few benefits:

- It makes it possible to re-use components and UI elements between projects
- Common styling can be implemented, especially in strongly typed UIs like XAML or Blazor
- UI components can be created that can be used to edit any entity that implements a common interface

## Provided Types

### Common Property Interfaces

Common property interfaces are provided to standardize the most common properties.

The full list of interfaces is available in the
[API documentation](https://github.ploch.dev/ploch-common/api/Ploch.Common.Data.Model.html).
