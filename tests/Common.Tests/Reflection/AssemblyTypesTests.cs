using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestAssembly1;
using Ploch.Common.Tests.TestAssembly2;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

[SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Part of this test is to test the non-generic overload")]
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Test code, no performance concern and readability is preferred")]
public class AssemblyTypesTests
{
    [Fact]
    public void GetImplementations_should_return_all_IBaseInterface_implementations_from_assembly_list()
    {
        TestGetConcreteImplementationsForIBaseInterface(AssemblyTypes.GetImplementations<IBaseInterface>(true, GetTestAssemblies()));
    }

    [Fact]
    public void GetImplementations_extension_method_should_return_all_IBaseInterface_implementations_from_assembly_list()
    {
        TestGetConcreteImplementationsForIBaseInterface(GetTestAssemblies().GetImplementations<IBaseInterface>());
    }

    [Fact]
    public void GetImplementations_should_return_all_BaseClass_implementations_from_assembly_list()
    {
        TestGetConcreteImplementationsForBaseClass(AssemblyTypes.GetImplementations<BaseClass>(true, GetTestAssemblies()));
    }

    [Fact]
    public void GetImplementations_extension_method_should_return_all_BaseClass_implementations_from_assembly_list()
    {
        TestGetConcreteImplementationsForBaseClass(GetTestAssemblies().GetImplementations<BaseClass>());
    }

    [Fact]
    public void GetImplementations_should_return_all_IGenericInterface_implementations_from_assembly_list()
    {
        TestGetImplementationsForOpenGenericIGenericInterfaceIncludingNotConcrete(AssemblyTypes.GetImplementations(typeof(IGenericInterface<>),
                                                                                                                   false,
                                                                                                                   GetTestAssemblies()));
    }

    [Fact]
    public void GetImplementations_extension_method_should_return_all_IGenericInterface_implementations_from_assembly_list()
    {
        TestGetImplementationsForOpenGenericIGenericInterfaceIncludingNotConcrete(GetTestAssemblies().GetImplementations(typeof(IGenericInterface<>), false));
    }

    [Fact]
    public void GetImplementations_generic_extension_method_should_return_all_IGenericInterface_implementations_from_assembly_list()
    {
        var appDomainImplementations = AssemblyTypes.GetAppDomainImplementations<IGenericInterface<int>>();
        var implementations = GetTestAssemblies().GetImplementations<IGenericInterface<int>>();

        ValidateImplementationTypes(implementations,
                                    appDomainImplementations,
                                    typeof(InheritedFromGenericClass2OfIntAndString),
                                    typeof(TestClass4GenericClass3OfInt));
    }

    [Fact]
    public void GetImplementations_should_return_all_IGenericInterface_int_implementations_from_assembly_list()
    {
        var implementations = AssemblyTypes.GetImplementations(typeof(IGenericInterface<int>), true, GetTestAssemblies());

        implementations.Should().HaveCount(2);
        implementations.Should().Contain(typeof(InheritedFromGenericClass2OfIntAndString));
        implementations.Should().Contain(typeof(TestClass4GenericClass3OfInt));
    }

    [Fact]
    public void GetImplementations_should_return_all_IGenericInterface_int_not_just_concrete_implementations_from_assembly_list()
    {
        TestGetImplementationsForOpenGenericIGenericInterfaceIncludingNotConcrete(AssemblyTypes.GetImplementations(typeof(IGenericInterface<>),
                                                                                                                   false,
                                                                                                                   GetTestAssemblies()));
    }

    [Fact]
    public void GetImplementations_extension_method_should_return_all_IGenericInterface_int_not_just_concrete_implementations_from_assembly_list()
    {
        TestGetImplementationsForOpenGenericIGenericInterfaceIncludingNotConcrete(GetTestAssemblies().GetImplementations(typeof(IGenericInterface<>), false));
    }

    [Fact]
    public void GetAppDomainImplementations_Generic_should_return_concrete_implementations_from_app_domain_assemblies()
    {
        TestGetConcreteImplementationsForBaseClass(GetTestAssemblies().GetImplementations<BaseClass>());
    }

    private static void TestGetConcreteImplementationsForIBaseInterface(IEnumerable<Type> actualImplementations)
    {
        var appDomainImplementations = AssemblyTypes.GetAppDomainImplementations(typeof(IBaseInterface));

        ValidateImplementationTypes(actualImplementations,
                                    appDomainImplementations,
                                    typeof(BaseClassImplementation1),
                                    typeof(BaseClassImplementation2),
                                    typeof(DerivedClass),
                                    typeof(InheritingClass1),
                                    typeof(Interface1Implementation1),
                                    typeof(Interface1Implementation2),
                                    typeof(Interface1Implementation3),
                                    typeof(Interface1Implementation4));
    }

    private static void TestGetImplementationsForOpenGenericIGenericInterfaceIncludingNotConcrete(IEnumerable<Type> actualImplementations)
    {
        var appDomainImplementations = AssemblyTypes.GetAppDomainImplementations(typeof(IGenericInterface<>), false);

        ValidateImplementationTypes(actualImplementations,
                                    appDomainImplementations,
                                    typeof(IGenericInterface2<,>),
                                    typeof(TestGenericClass3<>),
                                    typeof(GenericClass2<,>),
                                    typeof(InheritedFromGenericClass2OfIntAndString),
                                    typeof(TestClass4GenericClass3OfInt));
    }

    private static void TestGetConcreteImplementationsForBaseClass(IEnumerable<Type> actualImplementations)
    {
        var appDomainImplementations = AssemblyTypes.GetAppDomainImplementations(typeof(BaseClass));

        ValidateImplementationTypes(actualImplementations,
                                    appDomainImplementations,
                                    typeof(BaseClassImplementation1),
                                    typeof(BaseClassImplementation2),
                                    typeof(InheritingClass1),
                                    typeof(DerivedClass));
    }

    private static void ValidateImplementationTypes(IEnumerable<Type> actualTypes, params IEnumerable<Type> expectedTypes)
    {
        actualTypes.Should().HaveCount(expectedTypes.Count(), "Actual types should have the same count as expected types");
        actualTypes.Should().Contain(expectedTypes, "All expected types should be found in actual types");
    }

    private static void ValidateImplementationTypes(IEnumerable<Type> actualTypes,
                                                    IEnumerable<Type> appDomainActualTypes,
                                                    params IEnumerable<Type> expectedTypes)
    {
        ValidateImplementationTypes(actualTypes, expectedTypes);

        appDomainActualTypes.Should().HaveCount(expectedTypes.Count(), "AppDomain types should match expected types");
        appDomainActualTypes.Should().BeEquivalentTo(actualTypes, "AppDomain types should match actual types from provided assemblies");
    }

    private static IEnumerable<Assembly> GetTestAssemblies()
    {
        yield return TestAssembly1Info.Assembly;
        yield return TestAssembly2Info.Assembly;
    }
}
