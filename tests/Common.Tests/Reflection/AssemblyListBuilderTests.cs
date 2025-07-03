using FluentAssertions;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestAssembly1;
using Ploch.Common.Tests.TestAssembly2;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class AssemblyListBuilderTests
{
    private readonly AssemblyListBuilder _sut;

    public AssemblyListBuilderTests() => _sut = new AssemblyListBuilder();

    [Fact]
    public void AddAssembly_should_add_assembly_to_collection()
    {
        // Arrange
        var assembly = typeof(string).Assembly;

        // Act
        var result = _sut.AddAssembly(assembly);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().ContainSingle().Which.Should().BeSameAs(assembly);
    }

    [Fact]
    public void AddAssembly_should_throw_when_assembly_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddAssembly(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("assembly");
    }

    [Fact]
    public void AddAssemblies_should_add_multiple_assemblies()
    {
        // Arrange
        var assemblies = new[] { typeof(Assembly1Type).Assembly, typeof(Assembly2Type1).Assembly };

        // Act
        var result = _sut.AddAssemblies(assemblies);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().BeEquivalentTo(assemblies);
    }

    [Fact]
    public void AddAssemblies_should_throw_when_assemblies_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddAssemblies(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("assemblies");
    }

    [Fact]
    public void AddFromType_generic_should_add_assembly_from_type()
    {
        // Act
        var result = _sut.AddFromType<Assembly1Type>().AddFromType<Assembly2Type1>();

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().HaveCount(2).And.Contain([ typeof(Assembly1Type).Assembly, typeof(Assembly2Type1).Assembly ]);
    }

    [Fact]
    public void AddFromType_should_add_assembly_from_type()
    {
        // Arrange
        var type = typeof(string);

        // Act
        var result = _sut.AddFromType(type);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().ContainSingle().Which.Should().BeSameAs(type.Assembly);
    }

    [Fact]
    public void AddFromType_should_throw_when_type_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddFromType(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("type");
    }

    [Fact]
    public void AddFromTypes_should_add_assemblies_from_multiple_types()
    {
        // Arrange
        var types = new[] { typeof(Assembly1Type), typeof(Assembly2Type1) };

        // Act
        var result = _sut.AddFromTypes(types);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().BeEquivalentTo(types.Select(t => t.Assembly));
    }

    [Fact]
    public void AddFromTypes_should_throw_when_types_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddFromTypes(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("types");
    }

    [Fact]
    public void AddFromObject_should_add_assembly_from_object()
    {
        // Arrange
        var obj = "test string";

        // Act
        var result = _sut.AddFromObject(obj);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().ContainSingle().Which.Should().BeSameAs(obj.GetType().Assembly);
    }

    [Fact]
    public void AddFromObject_should_throw_when_object_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddFromObject(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("obj");
    }

    [Fact]
    public void AddFromObjects_should_only_add_unique_assebly_from_objects()
    {
        // Act
        var result = _sut.AddFromObjects("test string", 42);

        // Assert
        result.Should().BeSameAs(_sut);
        result.Build().Should().ContainSingle(assembly => assembly == typeof(string).Assembly);
    }

    [Fact]
    public void AddFromObjects_should_throw_when_objects_is_null()
    {
        // Act & Assert
        var action = () => _sut.AddFromObjects(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("objects");
    }

    [Fact]
    public void Build_should_return_unique_assemblies()
    {
        // Arrange
        var assembly = typeof(string).Assembly;
        _sut.AddAssembly(assembly).AddAssembly(assembly); // Add the same assembly twice

        // Act
        var result = _sut.Build();

        // Assert
        result.Should().ContainSingle().Which.Should().BeSameAs(assembly);
    }
}
