using System;
using Dawn;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.DawnGuard.Tests
{
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
            Type? nullArg = null;

            Action act = () => Guard.Argument(nullArg, nameof(nullArg))!.AssignableToOrNull(typeof(ITestService1));

            act.Should().NotThrow();
        }

        [Fact]
        public void AssignableToOrNull_generic_guard_should_not_throw_if_argument_is_null()
        {
            Type? nullArg = null;

#pragma warning disable CS8620
            Action act = () => Guard.Argument(nullArg, nameof(nullArg)).AssignableToOrNull<ITestService1>();
#pragma warning restore CS8620

            act.Should().NotThrow();
        }

#pragma warning disable SA1201 // Elements should appear in the correct order - this is a test class and order doesn't matter.
        private interface ITestService1
#pragma warning restore SA1201
        { }

        private interface ITestService2
        { }

        private class TestService1 : ITestService1
        { }

        private abstract class TestService2 : ITestService2
        { }

        private class TestService12 : TestService2, ITestService1
        { }
    }
}