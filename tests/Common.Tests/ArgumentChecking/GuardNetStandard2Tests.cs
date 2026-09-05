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

/// <summary>
///     Covers the assignability guards on the netstandard2.0 partial, which cannot auto-capture the
///     parameter name and so takes it explicitly. Mirrors GuardAssignableToNet7Tests so both partials
///     are held to the same behaviour (#347).
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules",
                                                 "SA1106:Code should not contain empty statements",
                                                 Justification = "Empty interfaces and classes are intentional test fixtures; matches the ported DawnGuard tests.")]
public class GuardAssignableToNetStandard2Tests
{
    [Fact]
    public void AssignableTo_should_not_throw_when_type_implements_the_target_interface()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableTo(typeof(ITestService1), nameof(serviceType));

        act.Should().NotThrow();
    }

    [Fact]
    public void AssignableTo_should_return_the_validated_argument_so_it_can_be_chained()
    {
        var serviceType = typeof(TestService12);

        var result = serviceType.AssignableTo(typeof(ITestService2), nameof(serviceType));

        result.Should().BeSameAs(serviceType);
    }

    [Fact]
    public void AssignableTo_should_not_throw_when_the_argument_is_the_target_type_itself()
    {
        // Reflexive, matching Type.IsAssignableFrom - see the net7 counterpart for the rationale.
        var serviceType = typeof(TestService1);

        var act = () => serviceType.AssignableTo(typeof(TestService1), nameof(serviceType));

        act.Should().NotThrow();
    }

    [Fact]
    public void AssignableTo_should_throw_ArgumentException_naming_the_parameter_and_target_type_when_not_assignable()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableTo(typeof(TestService1), nameof(serviceType));

        act.Should()
           .Throw<ArgumentException>()
           .Which.Message.Should()
           .Contain(typeof(TestService1).FullName)
           .And.Contain(nameof(serviceType));
    }

    [Fact]
    public void AssignableTo_generic_should_throw_ArgumentException_when_not_assignable()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableTo<TestService1>(nameof(serviceType));

        act.Should().Throw<ArgumentException>().Which.Message.Should().Contain(typeof(TestService1).FullName).And.Contain(nameof(serviceType));
    }

    [Fact]
    public void AssignableTo_generic_should_not_throw_when_type_implements_the_target_interface()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableTo<ITestService2>(nameof(serviceType));

        act.Should().NotThrow();
    }

    [Fact]
    public void AssignableTo_should_throw_ArgumentNullException_naming_the_parameter_when_argument_is_null()
    {
        Type? nullType = null;

        var act = () => nullType.AssignableTo(typeof(ITestService1), nameof(nullType));

        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(nullType));
    }

    [Fact]
    public void AssignableTo_should_throw_ArgumentNullException_when_the_target_type_is_null()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableTo(null!, nameof(serviceType));

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AssignableToOrNull_should_not_throw_when_argument_is_null()
    {
        Type? nullType = null;

        var act = () => nullType.AssignableToOrNull(typeof(ITestService1), nameof(nullType));

        act.Should().NotThrow();
    }

    [Fact]
    public void AssignableToOrNull_generic_should_not_throw_when_argument_is_null()
    {
        Type? nullType = null;

        var act = () => nullType.AssignableToOrNull<ITestService1>(nameof(nullType));

        act.Should().NotThrow();
    }

    [Fact]
    public void AssignableToOrNull_should_still_throw_when_a_non_null_argument_is_not_assignable()
    {
        var serviceType = typeof(TestService12);

        var act = () => serviceType.AssignableToOrNull(typeof(TestService1), nameof(serviceType));

        act.Should().Throw<ArgumentException>().Which.Message.Should().Contain(nameof(serviceType));
    }

    [Fact]
    public void AssignableToOrNull_should_return_null_when_argument_is_null()
    {
        Type? nullType = null;

        // Static-form invocation: extension-method syntax on a variable known to be null reads as a
        // null dereference to CodeQL/DeepSource, even though extension methods never dereference the
        // receiver. The call and the assertion are otherwise identical.
        var result = Guard.AssignableToOrNull(nullType, typeof(ITestService1), nameof(nullType));

        result.Should().BeNull();
    }

#pragma warning disable SA1201 // Test fixture types are declared after the tests that use them.
    private interface ITestService1;

    private interface ITestService2;

    private class TestService1 : ITestService1;

    // Not abstract: an abstract class with no members is an anti-pattern (DeepSource CS-R1078),
    // and abstractness is not load-bearing here - the fixture exists to give TestService12 a
    // class-inheritance leg alongside its interface, which a concrete base provides equally well.
    private class TestService2 : ITestService2;

    private sealed class TestService12 : TestService2, ITestService1;
#pragma warning restore SA1201
}
