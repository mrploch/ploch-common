using FluentAssertions;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Tests.TestTypes;

namespace Ploch.Common.Tests.ArgumentChecking;

public class GuardTests
{
    [Fact]
    public void RequiredFalse_should_not_throw_exception_when_condition_is_false()
    {
        var act = () => false.RequiredFalse("This message should not be seen");
        act.Should().NotThrow();
    }

    [Fact]
    public void RequiredFalse_should_throw_exception_when_condition_is_true()
    {
        var act = () => true.RequiredFalse("Condition is true");
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Condition is true");
    }

    [Fact]
    public void RequiredTrue_should_not_throw_exception_when_condition_is_true()
    {
        var act = () => true.RequiredTrue("This message should not be seen");
        act.Should().NotThrow();
    }

    [Fact]
    public void RequiredTrue_should_throw_exception_when_condition_is_false()
    {
        var act = () => false.RequiredTrue("Condition is false");
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Be("Condition is false");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Positive_should_not_throw_exception_when_number_is_positive(int argumentXyz)
    {
        var act = () => 1.Positive("This message should not be seen");
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

    [Fact]
    public void RequiredNotNull_should_throw_InvalidOperationException_if_argument_is_null()
    {
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull("This is the exception message for {0}", nameof(testClass));

        act.Should().Throw<InvalidOperationException>().WithMessage("This is the exception message for testClass");
    }
}
