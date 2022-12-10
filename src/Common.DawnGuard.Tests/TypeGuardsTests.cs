using System;
using Dawn;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.DawnGuard.Tests
{
    public interface ITestService1
    { }

    public interface ITestService2
    { }

    public class TestService1 : ITestService1
    { }

    public class TestService1A : TestService1
    { }

    public abstract class TestService2 : ITestService2
    { }

    public class TestService2A : TestService2
    { }

    public class TestService12 : TestService2, ITestService1
    { }

    public class TypeGuardsTests
    {
        [Fact]
        public void AssignableTo_guard_should_verify_that_type_argument_is_assignable_to_specified_type()
        {
            var testService12 = typeof(TestService12);

            Guard.Argument(testService12, nameof(testService12)).AssignableTo(typeof(ITestService1));
            Guard.Argument(testService12, nameof(testService12)).AssignableTo(typeof(ITestService2));

            Action failure1 = () => Guard.Argument(testService12, nameof(testService12)).AssignableTo(typeof(TestService1));
            failure1.Should().Throw<ArgumentException>().Which.Message.Should().Contain(typeof(TestService1).FullName).And.Contain(nameof(testService12));
        }

        [Fact]
        public void AssignableTo_generic_guard_should_verify_that_type_argument_is_assignable_to_specified_type()
        {
            var testService12 = typeof(TestService12);

            Guard.Argument(testService12, nameof(testService12)).AssignableTo<ITestService1>();
            Guard.Argument(testService12, nameof(testService12)).AssignableTo<ITestService2>();

            Action failure1 = () => Guard.Argument(testService12, nameof(testService12)).AssignableTo<TestService1>();
            failure1.Should().Throw<ArgumentException>().Which.Message.Should().Contain(typeof(TestService1).FullName).And.Contain(nameof(testService12));
        }

        [Fact]
        public void AssignableTo_guard_should_throw_ArgumentNullException_if_argument_is_null()
        {
            Type? nullArg = null;

            Action act = () => Guard.Argument(nullArg, nameof(nullArg))!.AssignableTo(typeof(ITestService1));

            act.Should().Throw<ArgumentNullException>().Which.Message.Should().Contain(nameof(nullArg));
        }

        [Fact]
        public void AssignableToOrNull_guard_should_not_throw_if_argument_is_null()
        {
            Type nullArg = null;

            Action act = () => Guard.Argument(nullArg, nameof(nullArg))!.AssignableToOrNull(typeof(ITestService1));

            act.Should().NotThrow();
        }

        [Fact]
        public void AssignableToOrNull_generic_guard_should_not_throw_if_argument_is_null()
        {
            Type nullArg = null;

#pragma warning disable CS8620
            Action act = () => Guard.Argument(nullArg, nameof(nullArg)).AssignableToOrNull<ITestService1>();
#pragma warning restore CS8620

            act.Should().NotThrow();
        }
    }
}