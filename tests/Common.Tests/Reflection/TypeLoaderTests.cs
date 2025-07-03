using System.Reflection;
using FluentAssertions;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestAssembly1;
using Ploch.Common.Tests.TestAssembly2;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class TypeLoaderTests
{
    [Theory]
    [InlineData(false,
                new[] { typeof(IBaseInterface) },
                new[] { typeof(DerivedClass), typeof(BaseClassImplementation1), typeof(Interface1Implementation1), typeof(Interface1Implementation2) })]
    [InlineData(true,
                new[] { typeof(IGenericInterface<>) },
                new[] { typeof(TypeHierarchies.HierarchyOne.IDerivedTestInterface), typeof(TestGenericClass3<>), typeof(TestClass4GenericClass3OfInt) })]
    public void
        LoadTypes_loads_specific_types_abstract_from_assemblyies_specified_by_configuration_including_assemblies_that_match_include_pattern_and_excluding_assemblies_that_match_exclude_pattern(
            bool includeAbstractTypes,
            IEnumerable<Type>? baseTypes,
            IEnumerable<Type> expectedTypes)
    {
        var loadedTypes = TypeLoader.Configure(tlc =>
                                               {
                                                   tlc.WithAssemblyGlob(globCfg => globCfg.AddInclude("Ploch.Common.*").AddExclude("*.TestAssembly2"))
                                                      .WithBaseType<TypeHierarchies.HierarchyOne.IBaseTestInterface>()
                                                      .WithBaseTypes(baseTypes?.ToArray() ?? [])
                                                      .IncludeAbstractTypes(includeAbstractTypes);
                                               })
                                    .LoadTypes<TypeHierarchies.HierarchyThree.AbstractBaseClass>()
                                    .LoadTypes<Assembly1Type>()
                                    .LoadTypes<Assembly2Type1>()
                                    .LoadedTypes;

        loadedTypes.Should()
                   .HaveCount(2 + expectedTypes.Count())
                   .And.Contain([ typeof(TypeHierarchies.HierarchyOne.BaseInterfaceImplementor),
                                  typeof(TypeHierarchies.HierarchyOne.DerivedInterfaceImplementor) ]);

        loadedTypes.Should().Contain(expectedTypes);
    }

    [Fact]
    public void LoadTypes_loads_specific_types_from_assemblies_containing_specified_types()
    {
        var typeLoader = TypeLoader.Configure(tlc => tlc.IncludeAbstractTypes().WithBaseTypes(typeof(IGenericInterface<>)))
                                   .LoadTypes(typeof(Assembly1Type), typeof(Assembly2Type1));

        typeLoader.LoadedTypes.Should().HaveCount(5);
        typeLoader.LoadedTypes.Should()
                  .Contain([ typeof(IGenericInterface2<,>),
                             typeof(TestGenericClass3<>),
                             typeof(GenericClass2<,>),
                             typeof(InheritedFromGenericClass2OfIntAndString),
                             typeof(TestClass4GenericClass3OfInt) ]);
    }

    [Fact]
    public void LoadTypes_loads_specific_types_using_glob_type_name_pattern()
    {
        var typeLoader = TypeLoader
                         .Configure(tlc => tlc.IncludeAbstractTypes()
                                              .WithBaseTypes(typeof(IBaseInterface))
                                              .WithTypeNameGlob(matcher => matcher.AddInclude("*Imple*").AddExclude("*tion2*")))
                         .LoadTypes(typeof(Assembly1Type), typeof(Assembly2Type1));

        typeLoader.LoadedTypes.Should().HaveCount(4);
        typeLoader.LoadedTypes.Should()
                  .Contain([ typeof(BaseClassImplementation1),
                             typeof(Interface1Implementation1),
                             typeof(Interface1Implementation3),
                             typeof(Interface1Implementation4) ]);
    }

    [Fact]
    public void Configure_throws_ArgumentNullException_when_configurator_is_null()
    {
        // Act
        Action act = () => TypeLoader.Configure(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("configurator");
    }

    [Fact]
    public void Constructor_throws_ArgumentNullException_when_configuration_is_null()
    {
        // Arrange
        var constructorInfo = typeof(TypeLoader).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                                                .First(c => c.GetParameters().Length == 1 &&
                                                            c.GetParameters()[0].ParameterType == typeof(TypeLoaderConfigurator));

        // Act
        Action act = () => TypeLoader.Configure(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("configurator");
    }
}
