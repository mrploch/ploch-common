using Ploch.Common.ArgumentChecking;
using Ploch.Common.Tests.Reflection;
using Ploch.Common.Tests.TestTypes;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.ArgumentChecking;

public class GuardNet7Tests
{
    [Fact]
    public void NotNullOrEmpty_should_not_throw_if_string_is_not_null_or_empty()
    {
        // Arrange
        var argument = "test";

        // Act
        var act = () => argument.NotNullOrEmpty();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void RequiredNotNull_should_throw_InvalidOperationException_when_argument_is_null_with_provided_message()
    {
        TestClass? obj = null;
        var action = () => obj.RequiredNotNull("Custom message for {0}");

        action.Should().Throw<InvalidOperationException>().WithMessage($"Custom message for {nameof(obj)}");
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
        var x = 5;
        var y = 10;

        // Act & Assert
        var act = () => condition.RequiredTrue("This message should not be seen");

        // Act & Assert
        var act1 = () => (x < y).RequiredTrue("Expression is false");
        act1.Should().NotThrow();
        (x < y).RequiredTrue("Expression {0} is false").Should().BeTrue();
        act.Should().NotThrow();
        condition.RequiredTrue("This message should not be seen").Should().BeTrue();
    }

    [Fact]
    public void RequiredTrue_should_throw_exception_when_condition_is_false_with_custom_message()
    {
        // Arrange
        var x = 5;
        var y = 10;

        // Act & Assert
        var act = () => (x > y).RequiredTrue("Expression {0} is false");
        act.Should().Throw<InvalidOperationException>().WithMessage("Expression x > y is false");
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

#if NET7_0_OR_GREATER
    [Fact]
    public void NotNull_should_return_value_if_argument_is_not_null()
    {
        // Arrange
        var expectedValue = new OwnedPropertyInfoTests.TestClass();

        // Act
        var actualValue = GuardForNotNull(expectedValue);

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Fact]
    public void NotNull_should_throw_if_argument_is_null_with_argument_name()
    {
        // Act
        var action = () => GuardForNotNull(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("*null*argumentName*").WithParameterName("argumentName");
    }

    [Fact]
    public void NotNull_should_throw_if_struct_argument_is_null_with_argument_name()
    {
        // Act
        var action = () => GuardForNotNullStruct(default);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("*null*argumentName*").WithParameterName("argumentName");
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_if_string_is_null()
    {
        // Arrange
        string? argXyz = null;

        // Act
        var act = () => argXyz.NotNullOrEmpty();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*null*Parameter*").WithParameterName(nameof(argXyz));
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_if_string_is_empty()
    {
        // Arrange
        var argXyz = string.Empty;

        // Act
        var act = () => argXyz.NotNullOrEmpty();

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*empty*string*").WithParameterName(nameof(argXyz));
    }

    [Fact]
    public void Required_should_test_the_expresssion_and_print_out_exception_message_with_member_and_expresssion()
    {
        var class1 = new Class1 { MyProperty = "test" };

        var action = () => class1.MyProperty.EndsWith("xyx", StringComparison.CurrentCulture).RequiredTrue();

        var str =
            $"Condition*class1.MyProperty.EndsWith*required*true*{nameof(Required_should_test_the_expresssion_and_print_out_exception_message_with_member_and_expresssion)}*";
        action.Should().Throw<InvalidOperationException>().WithMessage(str);
    }

    [Fact]
    public void NotOutOfRange_should_return_value_if_enum_is_defined()
    {
        // Arrange
        var expectedValue = DayOfWeek.Monday;

        // Act
        var result = expectedValue.NotOutOfRange();

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void NotOutOfRange_should_throw_when_enum_value_is_not_defined()
    {
        // Arrange
        var invalidValue = (DayOfWeek)99; // 99 is not a valid DayOfWeek value

        // Act
        var action = () => invalidValue.NotOutOfRange();

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
        byteEnumValue.NotOutOfRange().Should().Be(byteEnumValue);
        longEnumValue.NotOutOfRange().Should().Be(longEnumValue);

        // Invalid cases should throw
        var byteAction = () => invalidByteEnumValue.NotOutOfRange();
        byteAction.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidByteEnumValue}*not defined*{nameof(ByteEnum)}*");

        var longAction = () => invalidLongEnumValue.NotOutOfRange();
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
        singleFlag.NotOutOfRange().Should().Be(singleFlag);

        // Combined valid flags should pass
        combinedFlags.NotOutOfRange().Should().Be(combinedFlags);

        // Invalid flag should throw
        var action = () => invalidFlag.NotOutOfRange();
        action.Should().Throw<ArgumentOutOfRangeException>().WithMessage($"*{invalidFlag}*not defined*{nameof(TestEnumWithFlags)}*");
    }

    [Fact]
    public void NotNullOrDefault_should_return_value_if_argument_is_non_null_and_non_default()
    {
        // Arrange
        var argument = 42;

        // Act
        var result = argument.NotNullOrDefault();

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void NotNullOrDefault_should_return_reference_type_if_not_null_or_default()
    {
        // Arrange
        var argument = "valid string";

        // Act
        var result = argument.NotNullOrDefault();

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void NotNullOrDefault_should_throw_if_argument_is_null()
    {
        // Arrange
        string? argXyz = null;

        // Act
        Action act = () => argXyz.NotNullOrDefault();

        // Assert
        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(argXyz));
    }

    [Fact]
    public void NotNullOrDefault_should_throw_if_argument_is_default_value()
    {
        // Arrange
        int argXyz = default;

        // Act
        Action act = () => argXyz.NotNullOrDefault();

        // Assert
        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(argXyz));
    }

    [Fact]
    public void NotNullOrDefault_should_throw_with_correct_parameter_name_if_argument_is_null_or_default()
    {
        // Arrange
        string? nullArg = null;
        int defaultArg = default;

        // Act
        Action nullAct = () => nullArg.NotNullOrDefault();
        Action defaultAct = () => defaultArg.NotNullOrDefault();

        // Assert
        nullAct.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(nullArg));
        defaultAct.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == nameof(defaultArg));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Positive_should_not_throw_exception_when_number_is_positive(int argumentXyz)
    {
        var act = () => argumentXyz.Positive();
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
        var act = () => argumentXyz.Positive();
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(argumentXyz));
    }

    private static OwnedPropertyInfoTests.TestClass GuardForNotNull(OwnedPropertyInfoTests.TestClass argumentName)
    {
        var actualValue = argumentName.NotNull();

        return actualValue;
    }

    private static TestStruct GuardForNotNullStruct(TestStruct? argumentName)
    {
        var actualValue = argumentName.NotNull();

        return actualValue;
    }
#endif
}
