# Ploch.TestingSupport.XUnit3.AutoMoq

> AutoFixture + AutoMoq integration for xUnit v3: one attribute to generate test data and auto-create Moq mocks for all dependencies.

## Overview

`Ploch.TestingSupport.XUnit3.AutoMoq` removes the boilerplate of setting up `AutoFixture` with `AutoMoqCustomization` for every test class. It provides a single attribute — `[AutoMockData]` — that wires everything together: a fresh `Fixture` per test invocation, automatic generation of primitive values and populated objects, and Moq mocks for every interface or abstract class that appears as a constructor or method parameter.

The library also exposes the individual AutoFixture customizations used internally, so you can compose your own fixture when `AutoMockDataAttribute` does not match your exact needs.

Four customizations are included:

- **`AutoDataCommonCustomization`** — combines `AutoMoqCustomization`, `DoNotThrowOnRecursionCustomization`, and `OmitOnRecursionCustomization` into a single convenience customization. Optionally adds `IgnoreVirtualMembersCustomization`.
- **`DoNotThrowOnRecursionCustomization`** — removes AutoFixture's default `ThrowingRecursionBehavior` so that circular object graphs do not immediately raise an exception.
- **`OmitOnRecursionCustomization`** — adds `OmitOnRecursionBehavior` so that recursive branches are silently omitted instead of throwing.
- **`IgnoreVirtualMembersCustomization`** / **`IgnoreVirtualMembersSpecimenBuilder`** — instructs AutoFixture to omit virtual properties. Useful when the system under test has Moq-virtualized or EF Core lazy-loading proxy properties that must not be populated.

The project targets `net8.0` and depends on `Ploch.TestingSupport.XUnit3.Dependencies` (which transitively provides `AutoFixture.AutoMoq`, `AutoFixture.Xunit3`, `FluentAssertions`, and `Moq`).

## Installation

```shell
dotnet add package Ploch.TestingSupport.XUnit3.AutoMoq
```

## Key Types

| Type | Description |
|---|---|
| `AutoMockDataAttribute` | `[AutoDataAttribute]` preconfigured with `AutoDataCommonCustomization`. Apply to `[Theory]` methods to enable auto-generated test data and Moq mocks. Accepts `ignoreVirtualMembers` parameter. |
| `AutoDataCommonCustomization` | `ICustomization` that composes `AutoMoqCustomization`, `DoNotThrowOnRecursionCustomization`, and `OmitOnRecursionCustomization`. Use directly when you need a custom `Fixture`. |
| `DoNotThrowOnRecursionCustomization` | Removes `ThrowingRecursionBehavior` from the fixture's behaviour list. |
| `OmitOnRecursionCustomization` | Adds `OmitOnRecursionBehavior` to handle recursive graphs by omission. |
| `IgnoreVirtualMembersCustomization` | Adds `IgnoreVirtualMembersSpecimenBuilder` to the fixture. Accepts an optional `Type` to limit omission to a specific declaring type. |
| `IgnoreVirtualMembersSpecimenBuilder` | `ISpecimenBuilder` that returns `OmitSpecimen` for virtual properties, optionally filtered by `ReflectedType`. |

## Usage Examples

### Basic usage with `[AutoMockData]`

Decorate a `[Theory]` method with `[AutoMockData]`. AutoFixture generates the SUT and all dependencies. Interfaces and abstract classes arrive as Moq mocks.

```csharp
[Theory]
[AutoMockData]
public void ProcessOrder_should_call_repository(OrderService sut, Mock<IOrderRepository> repositoryMock)
{
    // Arrange
    var order = new Order { Id = 1, Status = OrderStatus.Pending };
    repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

    // Act
    sut.Process(1);

    // Assert
    repositoryMock.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once);
}
```

### Ignoring virtual members

Use the `ignoreVirtualMembers: true` parameter when the fixture populates EF Core navigation properties or Moq proxy members that should be left at their defaults:

```csharp
[Theory]
[AutoMockData(ignoreVirtualMembers: true)]
public void Entity_should_have_valid_name(Customer customer)
{
    // AutoFixture populated Name and other non-virtual properties.
    // Virtual navigation properties (e.g. Orders) were omitted.
    customer.Name.Should().NotBeNullOrEmpty();
}
```

### Composing a custom fixture

When you need more control than `[AutoMockData]` provides, build your own `Fixture` using `AutoDataCommonCustomization`:

```csharp
public class MyCustomAutoData : AutoDataAttribute
{
    public MyCustomAutoData() : base(() =>
    {
        var fixture = new Fixture();
        fixture.Customize(new AutoDataCommonCustomization(ignoreVirtualMembers: false));
        fixture.Register<ISpecialService>(() => new FakeSpecialService());
        return fixture;
    })
    { }
}
```

### Using individual customizations

Apply just the recursion-handling customizations to an existing fixture:

```csharp
var fixture = new Fixture()
    .Customize(new AutoMoqCustomization())
    .Customize(new DoNotThrowOnRecursionCustomization())
    .Customize(new OmitOnRecursionCustomization());
```

Limit virtual member omission to a single type:

```csharp
fixture.Customize(new IgnoreVirtualMembersCustomization(typeof(MyEntity)));
```

## Related Libraries

- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — The base xUnit v3 test helpers that this library depends on
- [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) — Meta-package that provides `AutoFixture.AutoMoq`, `AutoFixture.Xunit3`, `FluentAssertions`, and `Moq`
