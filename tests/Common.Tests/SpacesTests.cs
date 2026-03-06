namespace Ploch.Common.Tests;

public class SpacesTests
{
    [Fact]
    public void Spaces_should_create_a_string_with_the_specified_number_of_spaces()
    {
        // Act
        var result = Strings.Spaces(5);

        // Assert
        result.Should().Be("     ");
    }

    [Fact]
    public void Spaces_should_throw_an_exception_when_count_is_negative()
    {
        // Act
        Action act = () => Strings.Spaces(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("count");
    }

    [Fact]
    public void Underscores_should_create_a_string_with_the_specified_number_of_underscores()
    {
        // Act
        var result = Strings.Underscores(5);

        // Assert
        result.Should().Be("_____");
    }

    [Fact]
    public void Underscores_should_throw_an_exception_when_count_is_negative()
    {
        // Act
        Action act = () => Strings.Underscores(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("count");
    }

    [Fact]
    public void Dashes_should_create_a_string_with_the_specified_number_of_dashes()
    {
        // Act
        var result = Strings.Dashes(5);

        // Assert
        result.Should().Be("-----");
    }

    [Fact]
    public void Dashes_should_throw_an_exception_when_count_is_negative()
    {
        // Act
        Action act = () => Strings.Dashes(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("count");
    }

    [Fact]
    public void Dots_should_create_a_string_with_the_specified_number_of_dots()
    {
        // Act
        var result = Strings.Dots(5);

        // Assert
        result.Should().Be(".....");
    }

    [Fact]
    public void Dotss_should_throw_an_exception_when_count_is_negative()
    {
        // Act
        Action act = () => Strings.Dots(-1);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("count");
    }

    [Fact]
    public void String_generation_methods_should_handle_large_counts_without_exceptions()
    {
        // Arrange
        const int largeCount = 10000;

        // Act
        Action actSpaces = () => Strings.Spaces(largeCount);
        Action actUnderscores = () => Strings.Underscores(largeCount);
        Action actDashes = () => Strings.Dashes(largeCount);
        Action actDots = () => Strings.Dots(largeCount);

        // Assert
        actSpaces.Should().NotThrow();
        actUnderscores.Should().NotThrow();
        actDashes.Should().NotThrow();
        actDots.Should().NotThrow();

        // Additional verification of length
        Strings.Spaces(largeCount).Length.Should().Be(largeCount);
        Strings.Underscores(largeCount).Length.Should().Be(largeCount);
        Strings.Dashes(largeCount).Length.Should().Be(largeCount);
        Strings.Dots(largeCount).Length.Should().Be(largeCount);
    }
}
