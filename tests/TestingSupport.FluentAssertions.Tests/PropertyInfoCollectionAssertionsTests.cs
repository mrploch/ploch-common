namespace Ploch.TestingSupport.FluentAssertions.Tests;

public class PropertyInfoCollectionAssertionsTests
{
    [Fact]
    public void ContainProperty_should_fail_if_source_object_is_null()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();

        // Act
        var act = () => properties.Should().ContainProperties([ "test" ], null!);

        // Assert
        act.Should().Throw<XunitException>().Which.Message.Should().Be("You have to provide a source object that is not null");
    }

    [Fact]
    public void ContainProperty_should_fail_if_property_names_are_empty()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();

        // Act
        var act = () => properties.Should().ContainProperties([], new TestClass());

        // Assert
        act.Should().Throw<XunitException>().Which.Message.Should().Be("You have to provide at least one property name");
    }

    [Fact]
    public void ContainProperty_should_fail_if_property_names_are_empty_and_source_object_is_null()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();

        // Act
        var act = () => properties.Should().ContainProperties([], null!);

        // Assert
        act.Should().Throw<XunitException>().Which.Message.Should().Be("You have to provide at least one property name and a source object that is not null");
    }

    [Fact]
    public void ContainProperty_should_succeed_when_property_exists_in_collection()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();

        // Act
        var result = properties.Should().ContainProperty("Id", new TestClass());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ContainProperty_should_fail_when_property_does_not_exist_in_collection()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();

        // Act
        var act = () => properties.Should().ContainProperty("NonExistent", new TestClass());

        // Assert
        act.Should().Throw<XunitException>();
    }

    [Fact]
    public void ContainProperties_should_succeed_when_all_properties_exist_in_collection()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();
        var propertyNames = new[] { "Id", "Name" };

        // Act
        var result = properties.Should().ContainProperties(propertyNames, new TestClass());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ContainProperties_should_fail_when_any_property_does_not_exist_in_collection()
    {
        // Arrange
        var type = typeof(TestClass);
        var properties = type.GetProperties();
        var propertyNames = new[] { "Id", "NonExistent" };

        // Act
        var act = () => properties.Should().ContainProperties(propertyNames, new TestClass());

        // Assert
        act.Should().Throw<XunitException>();
    }

    private class TestClass
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
