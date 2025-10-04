using Ploch.Common.TypeConversion;

namespace Ploch.Common.Tests.TypeConversion;

public class EnumNameTests
{
    [Theory]
    [InlineData(null, null, true, true)]
    [InlineData(null, "", true, true)]
    [InlineData("test", "test", true, true)]
    [InlineData("test", "test", false, true)]
    [InlineData("test", "TEST", true, false)]
    [InlineData("test", "TEST", false, true)]
    public void Equals_should_return_correct_values_depending_on_options(string? enumNameString,
                                                                         string? comparisonString,
                                                                         bool caseSensitive,
                                                                         bool expectedResult)
    {
        var name = new EnumName(enumNameString, caseSensitive);

        var areEqual = name == comparisonString;

        areEqual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(null, null, true, false)]
    [InlineData("test", "test", true, false)]
    [InlineData("test", "test", false, false)]
    [InlineData("test", "TEST", true, true)]
    [InlineData("test", "TEST", false, false)]
    public void NotEquals_should_return_correct_values_depending_on_options(string? enumNameString,
                                                                            string? comparisonString,
                                                                            bool caseSensitive,
                                                                            bool expectedResult)
    {
        var name = new EnumName(enumNameString, caseSensitive);

        var areEqual = name != comparisonString;

        areEqual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("TEST", true)]
    [InlineData("xyz", true)]
    [InlineData("xyZ", false)]
    [InlineData("value1", false)]
    [InlineData("vAlUe1", true)]
    public void Equals_should_return_correct_values_when_used_by_Contains_method(string name, bool expectedResult)
    {
        var strings = new EnumName[] { "test", "TEST", new("vAlUe1", true), new("xyz", true) };

        strings.Contains(name).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("test")]
    [InlineData("TEST")]
    [InlineData("xyz")]
    [InlineData("xyZ")]
    [InlineData(null)]
    public void Explicit_operators_should_work_correctly(string? name)
    {
        EnumName enumName = name;

        enumName.Name.Should().Be(name);

        var enumNameString = (string)enumName;

        enumNameString.Should().Be(name);
    }
}
