using FluentAssertions;
using Ploch.Common.ArgumentChecking;
using Xunit;

namespace Ploch.Common.Net6.Tests.ArgumentChecking;

public class GuardTests
{
    [Fact]
    public void RequiredFalse_should_return_argument_when_false()
    {
        false.RequiredFalse("Should not throw").Should().BeFalse();
    }

    [Fact]
    public void RequiredFalse_should_throw_when_true()
    {
        Action act = () => true.RequiredFalse("Error message");
        act.Should().Throw<InvalidOperationException>().WithMessage("Error message");
    }

    [Fact]
    public void NotNull_should_return_value_for_non_null_struct()
    {
        int? value = 5;
        value.NotNull(nameof(value)).Should().Be(5);
    }

    [Fact]
    public void NotNull_should_throw_for_null_struct()
    {
        int? value = null;
        Action act = () => value.NotNull(nameof(value));
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(value));
    }

    [Fact]
    public void NotNull_should_return_value_for_non_null_reference()
    {
        var obj = "test";
        obj.NotNull(nameof(obj)).Should().Be("test");
    }

    [Fact]
    public void NotNull_should_throw_for_null_reference()
    {
        string? obj = null;
        Action act = () => obj.NotNull(nameof(obj));
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(obj));
    }

    [Fact]
    public void RequiredNotNull_should_return_value_for_non_null_struct()
    {
        int? value = 10;
        value.RequiredNotNull(nameof(value)).Should().Be(10);
    }

    [Fact]
    public void RequiredNotNull_should_throw_for_null_struct()
    {
        int? value = null;
        Action act = () => value.RequiredNotNull(nameof(value));
        act.Should().Throw<InvalidOperationException>().WithMessage("Variable value does not have value.");
    }

    [Fact]
    public void RequiredNotNull_should_return_value_for_non_null_reference()
    {
        var obj = new object();
        obj.RequiredNotNull(nameof(obj)).Should().Be(obj);
    }

    [Fact]
    public void RequiredNotNull_should_throw_for_null_reference_with_message()
    {
        object? obj = null;
        Action act = () => obj.RequiredNotNull(nameof(obj), "Custom error: {0}");
        act.Should().Throw<InvalidOperationException>().WithMessage("Custom error: obj");
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
        Action act = () => string.Empty.NotNullOrEmpty("str");
        act.Should().Throw<ArgumentException>().WithMessage("Argument cannot be null or empty.*");
    }

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
    public void RequiredNotNullOrEmpty_should_return_string_when_not_null_or_empty()
    {
        "abc".RequiredNotNullOrEmpty("str").Should().Be("abc");
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_throw_for_null_string_with_message()
    {
        string? str = null;
        Action act = () => str.RequiredNotNullOrEmpty(nameof(str), "Custom error: {0}");
        act.Should().Throw<InvalidOperationException>().WithMessage("Custom error: str");
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_throw_for_empty_string_with_default_message()
    {
        Action act = () => string.Empty.RequiredNotNullOrEmpty("str");
        act.Should().Throw<InvalidOperationException>().WithMessage("Variable str is empty.");
    }

    [Fact]
    public void NotOutOfRange_should_return_enum_value_when_defined()
    {
        TestEnum.A.NotOutOfRange(nameof(TestEnum.A)).Should().Be(TestEnum.A);
    }

    [Fact]
    public void NotOutOfRange_should_throw_for_undefined_enum_value()
    {
        Action act = () => ((TestEnum)99).NotOutOfRange("testEnum");
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("testEnum");
    }

    [Fact]
    public void Positive_should_return_value_when_positive()
    {
        5.Positive("val").Should().Be(5);
    }

    [Fact]
    public void Positive_should_throw_for_zero_or_negative()
    {
        Action act = () => 0.Positive("val");
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Value 0 is not positive.*");
    }

// “Enumeration type names should not have Flags or Enum suffixes”: this is perfectly valid for a test enum type like this.
#pragma warning disable S2344
    private enum TestEnum { A = 1, B = 2 }
#pragma warning restore SA1201
}
