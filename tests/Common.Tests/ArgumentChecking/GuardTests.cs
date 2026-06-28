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

    // Net7+ message-format semantics (single positional arg = format template) are tested in GuardNet7Tests.cs.
    // The netstandard2.0 partial inverts the parameter order (memberName first, message second), so the
    // single-positional-arg call has different semantics on that binary. See issue #207.
    [Fact]
    public void NotNullOrEmpty_should_return_enumerable_when_not_null_or_empty()
    {
        var list = new List<int> { 1 };
        list.NotNullOrEmpty(nameof(list)).Should().BeSameAs(list);
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_for_null_enumerable()
    {
        List<int>? list = null;
        Action act = () => list.NotNullOrEmpty(nameof(list));
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(list));
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_for_empty_enumerable()
    {
        var list = new List<int>();
        Action act = () => list.NotNullOrEmpty(nameof(list));
        act.Should().Throw<ArgumentException>().WithMessage("Argument cannot be null or empty.*");
    }

    [Fact]
    public void NotNullOrEmpty_should_return_string_when_not_null_or_empty()
    {
        "abc".NotNullOrEmpty("str").Should().Be("abc");
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_for_null_string()
    {
        string? str = null;
        Action act = () => str.NotNullOrEmpty(nameof(str));
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(str));
    }

    [Fact]
    public void NotNullOrEmpty_should_throw_for_empty_string()
    {
        // Both targets now raise the BCL message "The value cannot be an empty string." (issues #207, #211).
        Action act = () => string.Empty.NotNullOrEmpty("str");
        act.Should().Throw<ArgumentException>().WithMessage("The value cannot be an empty string.*").WithParameterName("str");
    }
}
