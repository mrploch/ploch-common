using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

public class TypeExtensionsTests
{
    public enum ExampleEnum
    {
        Value1,
        Value2
    }

    [Fact]
    public void IsConcreteImplementation_generic_should_return_true_for_concrete_classes_implementing_abstract_type()
    {
        // Arrange
        var concreteType = typeof(ConcreteExampleClass);

        // Act
        var isConcrete = concreteType.IsConcreteImplementation<AbstractExampleClass>();

        // Assert
        isConcrete.Should().BeTrue();
    }

    [Fact]
    public void IsConcreteImplementation_generic_should_return_true_for_concrete_classes_implementing_interface()
    {
        // Arrange
        var concreteType = typeof(ConcreteExampleClass);

        // Act
        var isConcrete = concreteType.IsConcreteImplementation<IExampleInterface>();

        // Assert
        isConcrete.Should().BeTrue();
    }

    [Fact]
    public void IsConcreteImplementation_should_return_true_for_concrete_classes_implementing_abstract_type()
    {
        // Arrange
        var concreteType = typeof(ConcreteExampleClass);

        // Act
#pragma warning disable CA2263
        var isConcrete = concreteType.IsConcreteImplementation(typeof(AbstractExampleClass));
#pragma warning restore CA2263

        // Assert
        isConcrete.Should().BeTrue();
    }

    [Fact]
    public void IsConcreteImplementation_should_return_true_for_concrete_classes_implementing_interface()
    {
        // Arrange
        var concreteType = typeof(ConcreteExampleClass);

        // Act
#pragma warning disable CA2263
        var isConcrete = concreteType.IsConcreteImplementation(typeof(IExampleInterface));
#pragma warning restore CA2263

        // Assert
        isConcrete.Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_array(int[] array) => array.GetType().IsEnumerable().Should().BeTrue();

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_enumerable(IEnumerable<string> list) => list.GetType().IsEnumerable().Should().BeTrue();

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_list(List<string> list) => list.GetType().IsEnumerable().Should().BeTrue();

    [Fact]
    public void IsImplementing_should_return_false_if_type_is_not_implementing_interface()
    {
        typeof(TestClass1).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
        typeof(ITestInterface4).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
    }

    [Theory]

    // Implementing
    [InlineData(typeof(TestClass1), typeof(IBaseInterface), true)]
    [InlineData(typeof(TestClass1), typeof(ITestInterface1), true)]
    [InlineData(typeof(TestClass1), typeof(ITestInterface2), true)]
    [InlineData(typeof(TestClass2), typeof(ITestInterface1), true)]
    [InlineData(typeof(TestClass2), typeof(ITestInterface2), true)]
    [InlineData(typeof(ITestInterface4), typeof(ITestInterface1), true)]
    [InlineData(typeof(ITestInterface4), typeof(ITestInterface2), true)]
    [InlineData(typeof(TestGenericClass3<>), typeof(IGenericInterface<>), true)]
    [InlineData(typeof(TestGenericClass3<int>), typeof(IGenericInterface<int>), true)]
    [InlineData(typeof(TestClass2), typeof(TestClass1), true)]
    [InlineData(typeof(TestClass4), typeof(TestGenericClass3<int>), true)]

    // Not implementing
    [InlineData(typeof(TestGenericClass3<int>), typeof(IGenericInterface<string>), false)]
    [InlineData(typeof(ITestInterface4), typeof(ITestInterface4), false)]
    [InlineData(typeof(TestClass1), typeof(ITestInterface3), false)]
    [InlineData(typeof(ITestInterface4), typeof(ITestInterface3), false)]
    public void IsImplementing_should_return_true_if_type_is_implementing_interface(Type type, Type baseInterface, bool result) =>
        type.IsImplementing(baseInterface).Should().Be(result);

    [Fact]
    public void IsNullable_should_return_false_if_type_is_not_nullable()
    {
        var intType = typeof(int);
        intType.IsNullable().Should().BeFalse();
    }

    [Fact]
    public void IsNullable_should_return_true_if_type_is_nullable()
    {
        var nullableIntType = typeof(int?);

        nullableIntType.IsNullable().Should().BeTrue();
    }

    [Fact]
    public void IsSimpleType_should_return_false_for_complex_types()
    {
        // Arrange
        var complexType = typeof(Class1);

        // Act
        var result = complexType.IsSimpleType();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(typeof(string), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(bool), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(char), true)]
    [InlineData(typeof(ExampleEnum), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(DateTime?), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(Guid?), true)]
    [InlineData(typeof(ExampleStruct), true)]
    [InlineData(typeof(ExampleStruct?), true)]
    [InlineData(typeof((string, int)), true)]
    [InlineData(typeof((string, int, TestClass1)), true)]
    [InlineData(typeof(IBaseInterface), false)]
    [InlineData(typeof(IExampleInterface), false)]
    [InlineData(typeof(ITestInterface1), false)]
    [InlineData(typeof(ITestInterface2), false)]
    [InlineData(typeof(ITestInterface3), false)]
    [InlineData(typeof(ITestInterface4), false)]
    [InlineData(typeof(TestClass1), false)]
    [InlineData(typeof(TestClass2), false)]
    [InlineData(typeof(TestGenericClass3<int>), false)]
    [InlineData(typeof(TestGenericClass3<string>), false)]
    [InlineData(typeof(TestClass4), false)]
    [InlineData(typeof(IList<string>), false)]
    [InlineData(typeof(ICollection<string>), false)]
    [InlineData(typeof(IReadOnlyCollection<string>), false)]
    [InlineData(typeof(IReadOnlyList<string>), false)]
    public void IsSimpleType_should_return_true_for_simple_types(Type type, bool expectedIsSimple)
    {
        // Act
        var result = type.IsSimpleType();

        // Assert
        result.Should().Be(expectedIsSimple);
    }

    public class ConcreteExampleClass : AbstractExampleClass
    { }

    public abstract class AbstractExampleClass : IExampleInterface
    { }

    public interface IExampleInterface : IBaseInterface
    { }

    public struct ExampleStruct
    { }

    public interface IBaseInterface
    { }

    public interface ITestInterface1 : IBaseInterface
    { }

    public interface ITestInterface2
    { }

    public interface ITestInterface3
    { }

    public class TestClass1 : ITestInterface1, ITestInterface2
    { }

    public interface ITestInterface4 : ITestInterface1, ITestInterface2
    { }

    public class TestClass2 : TestClass1
    { }

    public interface IGenericInterface<T>
    { }

    public class TestGenericClass3<T> : IGenericInterface<T>
    { }

    private class TestClass4 : TestGenericClass3<int>
    { }
}
