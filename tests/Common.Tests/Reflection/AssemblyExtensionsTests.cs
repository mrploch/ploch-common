using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Allowed in tests")]
public class AssemblyExtensionsTests
{
    private static Assembly TestTypesAssembly => typeof(TypeHierarchies.HierarchyNine.NestedGenericImplementor).Assembly;

    [Fact]
    public void GetAssemblyDirectory_should_return_exact_location_of_the_dll()
    {
        var assemblyDirectory = TestTypesAssembly.GetAssemblyDirectory();
        assemblyDirectory.Should().NotBeNullOrWhiteSpace();
        var currentDirectory = Directory.GetCurrentDirectory();
        assemblyDirectory.Should().Be(currentDirectory);
    }

    [Fact]
    public void GetTypesImplementing_should_return_only_non_abstract_types_implementing_base_type()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
#pragma warning disable CA2263 // This is a test case of non-generig method
        var implementingTypes = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchyTwo.IHierarchiesTestInterface));
#pragma warning restore CA2263

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.ConcreteClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyTwo.AbstractClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonImplementingClass));
    }

    [Fact]
    public void GetTypesImplementing_should_return_both_abstract_and_non_abstract_types_when_includeAbstract_is_true()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
#pragma warning disable CA2263 // This is a test case of non-generig method
        var implementingTypes = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchyTwo.IHierarchiesTestInterface), true);
#pragma warning restore CA2263

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.ConcreteClass_IHierarchiesTestInterface));
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.AbstractClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonImplementingClass));
    }

    [Fact]
    public void GetTypesImplementing_should_return_empty_collection_when_no_types_implement_base_type()
    {
        // Arrange
        var assembly = TestTypesAssembly;
        var nonImplementedInterface = typeof(TypeHierarchies.HierarchyTwo.INonImplementedInterface);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(nonImplementedInterface);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().BeEmpty();
    }

    [Fact]
    public void GetTypesImplementing_should_throw_ArgumentNullException_when_assembly_is_null()
    {
        // Arrange
        Assembly? nullAssembly = null;
        var baseType = typeof(ITestInterface);

        // Act & Assert
        Action act = () => nullAssembly!.GetTypesImplementing(baseType);

        act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("assembly");
    }

    [Fact]
    public void GetTypesImplementing_should_correctly_identify_types_implementing_interfaces()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Create a test interface hierarchy
        var baseInterfaceType = typeof(TypeHierarchies.HierarchyOne.IBaseTestInterface);
        var derivedInterfaceType = typeof(TypeHierarchies.HierarchyOne.IDerivedTestInterface);

        // Act
        var baseImplementingTypes = assembly.GetTypesImplementing(baseInterfaceType);
        var derivedImplementingTypes = assembly.GetTypesImplementing(derivedInterfaceType);

        // Assert
        baseImplementingTypes.Should().NotBeNull();
        derivedImplementingTypes.Should().NotBeNull();

        // Types implementing the base interface should include both direct and indirect implementors
        baseImplementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyOne.BaseInterfaceImplementor));
        baseImplementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyOne.DerivedInterfaceImplementor));

        // Types implementing the derived interface should only include direct implementors
        derivedImplementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyOne.DerivedInterfaceImplementor));
        derivedImplementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyOne.BaseInterfaceImplementor));
    }

    [Fact]
    public void GetTypesImplementing_should_correctly_identify_types_inheriting_from_abstract_classes()
    {
        // Arrange
        var assembly = TestTypesAssembly;
        var abstractBaseType = typeof(TypeHierarchies.HierarchyThree.AbstractBaseClass);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(abstractBaseType);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyThree.ConcreteChildClass));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyThree.AbstractBaseClass)); // Base class itself shouldn't be included
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonInheritingClass));
        implementingTypes.Should()
                         .NotContain(typeof(TypeHierarchies.HierarchyThree.AbstractChildClass)); // Abstract children are excluded when includeAbstract is false
    }

    [Fact]
    public void GetTypesImplementing_should_correctly_identify_types_implementing_generic_interfaces()
    {
        // Arrange
        var assembly = TestTypesAssembly;
        var genericInterfaceType = typeof(TypeHierarchies.HierarchyFour.IGenericTestInterface<>);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(genericInterfaceType);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyFour.GenericInterfaceImplementor));
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyFour.ConcreteGenericImplementor));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonGenericImplementor));
    }

    [Fact]
    public void GetTypesImplementing_should_not_return_the_base_type_itself()
    {
        // Arrange
        var assembly = TestTypesAssembly;
        var baseType = typeof(TypeHierarchies.HierarchyFive.BaseTypeClass);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(baseType);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyFive.DerivedTypeClass));
        implementingTypes.Should().NotContain(baseType); // The base type itself should not be included
    }

    [Fact]
    public void GetTypesImplementing_should_correctly_identify_nested_types_implementing_base_type()
    {
        // Arrange
        var assembly = TestTypesAssembly;
        var interfaceType = typeof(TypeHierarchies.HierarchySix.INestedTypeInterface);

        // Act
        var implementingTypes = assembly.GetTypesImplementing(interfaceType);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchySix.OuterClass.NestedImplementor));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchySix.OuterClass)); // Outer class doesn't implement the interface
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchySix.OuterClass.AbstractNestedImplementor)); // Abstract nested type is excluded
    }

    [Fact]
    public void GetTypesImplementing_should_correctly_differentiate_between_implementation_and_interface_contracts()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
#pragma warning disable CA2263 // This is a test case of non-generig method
        var contractTypes = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchySeven.IContract));
        var implementationTypes = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchyEight.IImplementation));
#pragma warning restore CA2263

        // Assert
        contractTypes.Should().NotBeNull();
        implementationTypes.Should().NotBeNull();

        // Contract implementors should only be those directly implementing IContract
        contractTypes.Should().Contain(typeof(TypeHierarchies.HierarchySeven.ContractImplementor));
        contractTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyEight.ImplementationImplementor));

        // Implementation implementors should only be those directly implementing IImplementation
        implementationTypes.Should().Contain(typeof(TypeHierarchies.HierarchyEight.ImplementationImplementor));
        implementationTypes.Should().NotContain(typeof(TypeHierarchies.HierarchySeven.ContractImplementor));
    }

    [Fact]
    public void GetTypesImplementing_Generic_should_return_only_non_abstract_types_implementing_base_type()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
        var implementingTypes = assembly.GetTypesImplementing<TypeHierarchies.HierarchyTwo.IHierarchiesTestInterface>();

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.ConcreteClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyTwo.AbstractClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonImplementingClass));
    }

    [Fact]
    public void GetTypesImplementing_Generic_should_return_both_abstract_and_non_abstract_types_when_includeAbstract_is_true()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
        var implementingTypes = assembly.GetTypesImplementing<TypeHierarchies.HierarchyTwo.IHierarchiesTestInterface>(true);

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.ConcreteClass_IHierarchiesTestInterface));
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyTwo.AbstractClass_IHierarchiesTestInterface));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonImplementingClass));
    }

    [Fact]
    public void GetTypesImplementing_Generic_should_handle_nested_generic_type_constraints_properly()
    {
        // Arrange
        var assembly = typeof(TypeHierarchies.HierarchyNine.NestedGenericImplementor).Assembly;

        // Act
        var implementingTypes = assembly.GetTypesImplementing<TypeHierarchies.HierarchyNine.INestedGenericInterface<List<string>>>();

        // Assert
        implementingTypes.Should().NotBeNull();
        implementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyNine.NestedGenericImplementor));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyNine.DifferentNestedGenericImplementor));
        implementingTypes.Should().NotContain(typeof(TypeHierarchies.NonImplementingClass));

        // Additional test with a different generic parameter
        var otherImplementingTypes = assembly.GetTypesImplementing<TypeHierarchies.HierarchyNine.INestedGenericInterface<Dictionary<string, int>>>();
        otherImplementingTypes.Should().Contain(typeof(TypeHierarchies.HierarchyNine.DifferentNestedGenericImplementor));
        otherImplementingTypes.Should().NotContain(typeof(TypeHierarchies.HierarchyNine.NestedGenericImplementor));
    }

    [Fact]
    public void GetTypesImplementing_TBaseType_should_work_correctly_with_open_generic_interfaces()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
        var implementingTypes = assembly.GetTypesImplementing<TypeHierarchies.HierarchyFour.IGenericTestInterface<object>>();

        // Assert
        implementingTypes.Should().NotBeNull();

        // Since we're looking for IGenericTestInterface<object> specifically, none of our test classes implement this exact type
        implementingTypes.Should().BeEmpty();

        // But if we use the non-generic overload with the open generic interface, we should get all implementations
        var openGenericImplementors = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchyFour.IGenericTestInterface<>));
        openGenericImplementors.Should().Contain(typeof(TypeHierarchies.HierarchyFour.GenericInterfaceImplementor));
        openGenericImplementors.Should().Contain(typeof(TypeHierarchies.HierarchyFour.ConcreteGenericImplementor));
    }

    [Fact]
    public void GetTypesImplementing_Generic_should_respect_class_constraint()
    {
        // Arrange
        var assembly = TestTypesAssembly;

        // Act
        // We're using ReferenceClass as our TBaseType which satisfies the class constraint
        var genericResult = assembly.GetTypesImplementing<TypeHierarchies.HierarchyTen.ReferenceClass>();
#pragma warning disable CA2263 // This is a test case of non-generic method
        var nonGenericResult = assembly.GetTypesImplementing(typeof(TypeHierarchies.HierarchyTen.ReferenceClass));
#pragma warning restore CA2263

        // Assert
        genericResult.Should().NotBeNull();
        genericResult.Should().BeEquivalentTo(nonGenericResult);
        genericResult.Should().Contain(typeof(TypeHierarchies.HierarchyTen.DerivedReferenceClass));
        genericResult.Should().NotContain(typeof(TypeHierarchies.HierarchyTen.ReferenceClass)); // Base type itself should not be included

        // Note: We can't directly test that a struct/value type doesn't compile as TBaseType
        // since that would be a compile-time error, not a runtime error
    }

    // Test types for the test
#pragma warning disable SA1201
#pragma warning disable S2094
#pragma warning disable RCS1102
#pragma warning disable S2326

// ReSharper disable once UnusedTypeParameter

#pragma warning restore SA1201
#pragma warning restore S2094
#pragma warning restore RCS1102
#pragma warning restore S2326
}
