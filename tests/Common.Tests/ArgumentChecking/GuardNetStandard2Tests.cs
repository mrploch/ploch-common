// Tests asserting the netstandard2.0 partial of Guard semantics.
// Compiled only on the net8.0 leg of Common.Tests (which loads the netstandard2.0 binary
// of Ploch.Common via SetTargetFramework on the ProjectReference).
// The corresponding net7+ semantics are covered in GuardNet7Tests.cs.
// See issues #207 (multi-target test coverage), #210 (parameter-order alignment) and #211 (message-text alignment).
//
// After #210 the netstandard2.0 partial shares the net7+ parameter order (messageFormat, memberName).
// The only residual difference is that netstandard2.0 cannot auto-capture memberName
// (no CallerArgumentExpression), so these tests pass it explicitly to reach parity with GuardNet7Tests.
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Tests.TestTypes;

// ReSharper disable MissingXmlDoc
namespace Ploch.Common.Tests.ArgumentChecking;

public class GuardNetStandard2Tests
{
    [Fact]
    public void RequiredNotNull_class_should_format_message_with_messageFormat_first_signature()
    {
        // (messageFormat, memberName) order, matching the net7+ partial (issue #210).
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull("Custom message for {0}", nameof(testClass));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Custom message for {nameof(testClass)}");
    }

    [Fact]
    public void RequiredNotNull_class_should_use_default_message_when_memberName_supplied()
    {
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull(memberName: nameof(testClass));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Variable {nameof(testClass)} cannot be null.");
    }

    [Fact]
    public void RequiredNotNull_class_should_use_empty_member_name_when_not_supplied()
    {
        // netstandard2.0 cannot auto-capture the member name (no CallerArgumentExpression),
        // so omitting it leaves the {0} placeholder empty. The net7+ build auto-captures it instead.
        TestClass? testClass = null;

        var act = () => testClass.RequiredNotNull();

        act.Should().Throw<InvalidOperationException>().WithMessage("Variable  cannot be null.");
    }

    [Fact]
    public void RequiredNotNull_class_should_return_argument_when_not_null()
    {
        var testClass = new TestClass();

        var result = testClass.RequiredNotNull(memberName: nameof(testClass));

        result.Should().BeSameAs(testClass);
    }

    [Fact]
    public void RequiredNotNull_struct_should_format_message_with_messageFormat_first_signature()
    {
        int? value = null;

        var act = () => value.RequiredNotNull("Custom message for {0}", nameof(value));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Custom message for {nameof(value)}");
    }

    [Fact]
    public void RequiredNotNull_struct_should_use_default_message_when_memberName_supplied()
    {
        int? value = null;

        var act = () => value.RequiredNotNull(memberName: nameof(value));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Variable {nameof(value)} cannot be null.");
    }

    [Fact]
    public void RequiredNotNull_struct_should_return_value_when_not_null()
    {
        int? value = 123;

        var result = value.RequiredNotNull(memberName: nameof(value));

        result.Should().Be(123);
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_format_message_with_messageFormat_first_signature()
    {
        var argument = string.Empty;

        var act = () => argument.RequiredNotNullOrEmpty("Empty value for {0}", nameof(argument));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Empty value for {nameof(argument)}");
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_use_default_message_when_memberName_supplied()
    {
        var argument = string.Empty;

        var act = () => argument.RequiredNotNullOrEmpty(memberName: nameof(argument));

        act.Should().Throw<InvalidOperationException>().WithMessage($"Variable {nameof(argument)} cannot be empty.");
    }

    [Fact]
    public void RequiredNotNullOrEmpty_should_return_argument_when_not_empty()
    {
        var argument = "valid";

        var result = argument.RequiredNotNullOrEmpty(memberName: nameof(argument));

        result.Should().Be(argument);
    }
}
