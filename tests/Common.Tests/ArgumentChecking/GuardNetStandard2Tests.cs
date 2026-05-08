// Tests asserting the netstandard2.0 partial of Guard semantics.
// Compiled only on the net8.0 leg of Common.Tests (which loads the netstandard2.0 binary
// of Ploch.Common via SetTargetFramework on the ProjectReference).
// The corresponding net7+ semantics are covered in GuardNet7Tests.cs.
// See issues #207 (multi-target test coverage) and #210 (the parameter-order divergence).
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Tests.TestTypes;

// ReSharper disable MissingXmlDoc
namespace Ploch.Common.Tests.ArgumentChecking;

public class GuardNetStandard2Tests
{
    [Fact]
    public void RequiredNotNull_should_format_message_with_memberName_first_signature()
    {
        // The netstandard2.0 partial signature is (argument, memberName, message=null).
        // Two positional args therefore mean (memberName, messageFormat).
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull(nameof(testClass), "Custom message for {0}");

        act.Should().Throw<InvalidOperationException>().WithMessage($"Custom message for {nameof(testClass)}");
    }

    [Fact]
    public void RequiredNotNull_should_use_default_message_when_only_memberName_supplied()
    {
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull(nameof(testClass));

        act.Should().Throw<InvalidOperationException>().WithMessage($"*{nameof(testClass)}*null*");
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_format_message_with_memberName_first_signature()
    {
        var argument = string.Empty;

        var act = () => argument.RequiredNotNullOrEmpty(nameof(argument), "Empty value for {0}");

        act.Should().Throw<InvalidOperationException>().WithMessage($"Empty value for {nameof(argument)}");
    }

    [Fact]
    public void RequiredNotNull_should_return_argument_when_not_null()
    {
        var testClass = new TestClass();

        var result = testClass.RequiredNotNull(nameof(testClass));

        result.Should().BeSameAs(testClass);
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_return_argument_when_not_empty()
    {
        var argument = "valid";

        var result = argument.RequiredNotNullOrEmpty(nameof(argument));

        result.Should().Be(argument);
    }
}
