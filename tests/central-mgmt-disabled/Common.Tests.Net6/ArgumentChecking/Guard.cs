using FluentAssertions;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Tests.TestTypes;
using Ploch.Common.Tests.TestTypes.TestingTypes;

// ReSharper disable MissingXmlDoc
namespace Ploch.Common.Tests.Net6.ArgumentChecking;

public class Guard
{
    [Fact]
    public void NotNull_should_return_value_if_argument_is_not_null()
    {
        // Arrange
        var expectedValue = new TestClass();

        // Act
        var actualValue = GuardForNotNull(expectedValue);

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public void NotNull_should_return_value_if_struct_argument_is_not_null()
    {
        // Arrange
        var expectedValue = new TestStruct(123, new TestStruct2(321, "test"));

        // Act
        var actualValue = GuardForNotNullStruct(expectedValue);

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public void NotNull_should_throw_if_argument_is_null_with_argument_name()
    {
        // Act
        var action = () => GuardForNotNull(null);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("*null*argumentName*").WithParameterName("argumentName");
    }

    [Fact]
    public void NotNull_should_throw_if_struct_argument_is_null_with_argument_name()
    {
        // Act
        var action = () => GuardForNotNullStruct(default!);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("*null*argumentName*").WithParameterName("argumentName");
    }

    [Fact]
    public void RequiredNotNull_should_throw_InvalidOperationException_if_member_and_message_are_null()
    {
        TestClass? obj = null;
        var action = () => obj.RequiredNotNull(null!);

        action.Should().Throw<InvalidOperationException>().WithMessage("*memberName*message*null*");
    }

    [Fact]
    public void MyMethod_should_explain()
    {
        object? obj = null;
        var b1 = false;

        var act = () => obj.RequiredNotNull(nameof(obj), "Custom message for {0}");
        var act2 = () => b1.RequiredFalse(nameof(b1));
        act.Should().Throw<InvalidOperationException>();
        act2.Should().NotThrow();
    }

    [Fact]
    public void RequiredNotNull_should_throw_InvalidOperationException_when_argument_is_null()
    {
        TestClass? obj = null;
        var action = () => obj.RequiredNotNull(nameof(obj));

        action.Should().Throw<InvalidOperationException>().WithMessage($"*{nameof(obj)}*null*");
    }

    [Fact]
    public void RequiredNotNull_should_throw_InvalidOperationException_when_argument_is_null_with_provided_message()
    {
        TestClass? obj = null;
        var action = () => obj.RequiredNotNull(nameof(obj), "Custom message for {0}");

        action.Should().Throw<InvalidOperationException>().WithMessage($"Custom message for {nameof(obj)}");
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_if_string_is_null()
    {
        // Arrange
        string? argXyz = null;

        // Act
        var act = () => argXyz.NotNullOrEmpty(nameof(argXyz));

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*null*Parameter*").WithParameterName(nameof(argXyz));
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_if_string_is_empty()
    {
        // Arrange
        var argXyz = string.Empty;

        // Act
        var act = () => argXyz.NotNullOrEmpty(nameof(argXyz));

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*empty**").WithParameterName(nameof(argXyz));
    }

    [Fact]
    public void NotNullOrEmpty_should_not_throw_if_string_is_not_null_or_empty()
    {
        // Arrange
        var argument = "test";

        // Act
        var act = () => argument.NotNullOrEmpty(nameof(argument));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void RequiredTrue_should_throw_exception_when_condition_is_false()
    {
        // Act & Assert
        var act = () => false.RequiredTrue("Condition is false");

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Condition is false");
    }

    [Fact]
    public void RequiredTrue_should_not_throw_exception_when_condition_is_true()
    {
        // Arrange
        var condition = true;

        // Act & Assert
        var act = () => condition.RequiredTrue("This message should not be seen");

        act.Should().NotThrow();
        condition.RequiredTrue("This message should not be seen").Should().BeTrue();
    }

    [Fact]
    public void RequiredTrue_should_work_with_boolean_expressions()
    {
        // Arrange
        var x = 5;
        var y = 10;

        // Act & Assert
        var act1 = () => (x < y).RequiredTrue("Expression is false");
        act1.Should().NotThrow();
        (x < y).RequiredTrue("Expression is false").Should().BeTrue();

        var act2 = () => (x > y).RequiredTrue("Expression is false");
        act2.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Expression is false");
    }

    [Fact]
    public void RequiredTrue_should_correctly_propagate_return_value_when_condition_is_true()
    {
        // Arrange
        var originalValue = true;

        // Act
        var result = originalValue.RequiredTrue("This message should not be seen");

        // Assert
        result.Should().BeTrue();
        result.Should().Be(originalValue);
    }

    [Fact]
    public void RequiredFalse_should_work_with_variable_condition_values()
    {
        // Arrange
        var trueCondition = true;
        var falseCondition = false;

        // Act & Assert
        var act1 = () => falseCondition.RequiredFalse("This message should not be seen");
        act1.Should().NotThrow();
        falseCondition.RequiredFalse("This message should not be seen").Should().BeFalse();

        var act2 = () => trueCondition.RequiredFalse("Condition is true");
        act2.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Condition is true");
    }

    [Fact]
    public void Required_should_test_the_expresssion_and_print_out_exception_message_with_member_and_expresssion()
    {
        var class1 = new Class1 { MyProperty = "test" };

        var action = () => class1.MyProperty.EndsWith("xyx", StringComparison.CurrentCulture).RequiredTrue("message");

        action.Should().Throw<InvalidOperationException>().WithMessage("message");
    }

    [Fact]
    public void NotOutOfRange_should_return_value_if_enum_is_defined()
    {
        // Arrange
        var expectedValue = DayOfWeek.Monday;

        // Act
        var result = expectedValue.NotOutOfRange(nameof(expectedValue));

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void NotOutOfRange_should_throw_when_enum_value_is_not_defined()
    {
        // Arrange
        var invalidValue = (DayOfWeek)99; // 99 is not a valid DayOfWeek value

        // Act
        var action = () => invalidValue.NotOutOfRange(nameof(invalidValue));

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidValue}*not defined*{nameof(DayOfWeek)}*");
    }

    [Fact]
    public void NotOutOfRange_should_handle_enums_with_different_underlying_types()
    {
        // Arrange
        var byteEnumValue = ByteEnum.Value1;
        var longEnumValue = LongEnum.Value1;

        var invalidByteEnumValue = (ByteEnum)200; // Value not defined in the enum
        var invalidLongEnumValue = (LongEnum)999; // Value not defined in the enum

        // Act & Assert
        // Valid cases should return the original value
        byteEnumValue.NotOutOfRange(nameof(byteEnumValue)).Should().Be(byteEnumValue);
        longEnumValue.NotOutOfRange(nameof(longEnumValue)).Should().Be(longEnumValue);

        // Invalid cases should throw
        var byteAction = () => invalidByteEnumValue.NotOutOfRange(nameof(invalidByteEnumValue));
        byteAction.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidByteEnumValue}*not defined*{nameof(ByteEnum)}*");

        var longAction = () => invalidLongEnumValue.NotOutOfRange(nameof(invalidLongEnumValue));
        longAction.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidLongEnumValue}*not defined*{nameof(LongEnum)}*");
    }

    // bug: https://github.com/mrploch/ploch-common/issues/159
    [Fact(Skip = "Handling of flags is currently not supported.")]
    public void NotOutOfRange_should_work_with_flags_enum_combined_values()
    {
        // Arrange
        var singleFlag = TestEnumWithFlags.FirstValue;
        var combinedFlags = TestEnumWithFlags.FirstValue | TestEnumWithFlags.SecondValue;
        var invalidFlag = (TestEnumWithFlags)128; // Value not defined in the enum

        // Act & .T
        // Single valid flag should pass
        singleFlag.NotOutOfRange(nameof(singleFlag)).Should().Be(singleFlag);

        // Combined valid flags should pass
        combinedFlags.NotOutOfRange(nameof(combinedFlags)).Should().Be(combinedFlags);

        // Invalid flag should throw
        var action = () => invalidFlag.NotOutOfRange(nameof(invalidFlag));
        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidFlag}*not defined*{nameof(TestEnumWithFlags)}*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Positive_should_not_throw_exception_when_number_is_positive(int argumentXyz)
    {
        var act = () => argumentXyz.Positive(nameof(argumentXyz));
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.1)]
    [InlineData(int.MinValue)]
    [InlineData(double.MinValue)]
    [InlineData(char.MinValue)]
    public void Positive_should_throw_exception_when_number_is_negative(double argumentXyz)
    {
        var act = () => argumentXyz.Positive(nameof(argumentXyz));
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(argumentXyz));
    }

    private static TestClass GuardForNotNull(TestClass? argumentName)
    {
        var actualValue = argumentName.NotNull(nameof(argumentName));

        return actualValue;
    }

    private static TestStruct GuardForNotNullStruct(TestStruct? argumentName)
    {
        var actualValue = argumentName.NotNull(nameof(argumentName));

        return actualValue;
    }
}
