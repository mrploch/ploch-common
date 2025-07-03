using System.Reflection;
using Ploch.Common.AssemblyLoading;

namespace Ploch.Common;

public class AppDomainTypesLoaderTests
{
    [Fact]
    public void Constructor_should_initialize_with_null_matchers_when_no_configuration()
    {
        // Arrange & Act
        var config = new TypeLoadingConfiguration();
        var loader = new AppDomainTypesLoader(config);

        // Assert
        loader.LoadedTypes.Should().BeEmpty();
    }

    [Fact]
    public void GetTypesImplementing_should_return_types_assignable_to_baseType()
    {
        // Arrange
        var config = new TypeLoadingConfiguration(BaseTypes: [ typeof(BaseType) ]);
        var loader = new AppDomainTypesLoader(config);

        // Simulate loading types
        var field = typeof(AppDomainTypesLoader).GetField("_types", BindingFlags.NonPublic | BindingFlags.Instance);
        var typesSet = (HashSet<Type>)field!.GetValue(loader)!;
        typesSet.Add(typeof(DerivedType));
        typesSet.Add(typeof(UnrelatedType));

        // Act
        var result = loader.GetTypesImplementing(typeof(BaseType)).ToList();

        // Assert
        result.Should().Contain(typeof(DerivedType));
        result.Should().NotContain(typeof(UnrelatedType));
    }

    [Fact]
    public void GetTypesImplementingT_should_return_types_assignable_to_generic_baseType()
    {
        // Arrange
        var config = new TypeLoadingConfiguration(BaseTypes: [ typeof(BaseType) ]);
        var loader = new AppDomainTypesLoader(config);

        // Simulate loading types
        var field = typeof(AppDomainTypesLoader).GetField("_types", BindingFlags.NonPublic | BindingFlags.Instance);
        var typesSet = (HashSet<Type>)field!.GetValue(loader)!;
        typesSet.Add(typeof(DerivedType));
        typesSet.Add(typeof(UnrelatedType));

        // Act
        var result = loader.GetTypesImplementing<BaseType>().ToList();

        // Assert
        result.Should().Contain(typeof(DerivedType));
        result.Should().NotContain(typeof(UnrelatedType));
    }

    [Fact]
    public void LoadedTypes_should_be_empty_initially()
    {
        // Arrange
        var loader = new AppDomainTypesLoader(new TypeLoadingConfiguration());

        // Act & Assert
        loader.LoadedTypes.Should().BeEmpty();
    }

    [Fact]
    public void ProcessAllAssemblies_should_not_throw()
    {
        // Arrange
        var loader = new AppDomainTypesLoader(new TypeLoadingConfiguration());

        // Act
        var act = () => loader.ProcessAllAssemblies();

        // Assert
        act.Should().NotThrow();
    }

    private class BaseType
    { }

    private class DerivedType : BaseType
    { }

    private class UnrelatedType
    { }
}
