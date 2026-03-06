using Moq;
using Ploch.Common.Matchers;

namespace Ploch.Common.Tests.Matchers;

public class PropertyMatcherTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMatch_should_return_result_from_string_matcher(bool isMatchResult)
    {
        // Arrange
        var mockStringMatcher = new Mock<IMatcher<string?>>(MockBehavior.Strict);
        var testString = "test value";
        var testObject = new { Name = testString };

        mockStringMatcher.Setup(m => m.IsMatch(testString)).Returns(isMatchResult);

        var propertyMatcher = new PropertyMatcher<object>(obj => ((dynamic)obj!).Name, mockStringMatcher.Object);

        // Act
        var result = propertyMatcher.IsMatch(testObject);

        // Assert
        result.Should().Be(isMatchResult);
        mockStringMatcher.Verify(m => m.IsMatch(testString), Times.Once);
    }

    [Fact]
    public void IsMatch_should_handle_null_property_values()
    {
        // Arrange
        var mockStringMatcher = new Mock<IMatcher<string?>>(MockBehavior.Strict);
        var testObject = new { Name = (string)null };

        mockStringMatcher.Setup(m => m.IsMatch(null)).Returns(true);

        var propertyMatcher = new PropertyMatcher<object>(obj => ((dynamic)obj!).Name, mockStringMatcher.Object);

        // Act
        var result = propertyMatcher.IsMatch(testObject);

        // Assert
        result.Should().BeTrue();
        mockStringMatcher.Verify(m => m.IsMatch(null), Times.Once);
    }

    [Fact]
    public void IsMatch_should_correctly_extract_property_using_provided_selector()
    {
        // Arrange
        var mockStringMatcher = new Mock<IMatcher<string?>>(MockBehavior.Strict);
        var testObject1 = new { FirstName = "John" };
        var testObject2 = new { FirstName = "Jane" };

        mockStringMatcher.Setup(m => m.IsMatch("John")).Returns(true);
        mockStringMatcher.Setup(m => m.IsMatch("Jane")).Returns(false);

        var propertyMatcher = new PropertyMatcher<object>(obj => ((dynamic)obj)!.FirstName, mockStringMatcher.Object);

        // Act
        var result1 = propertyMatcher.IsMatch(testObject1);
        var result2 = propertyMatcher.IsMatch(testObject2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
        mockStringMatcher.Verify(m => m.IsMatch("John"), Times.Once);
        mockStringMatcher.Verify(m => m.IsMatch("Jane"), Times.Once);
    }
}
