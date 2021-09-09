using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.ConsoleApplication.Core;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class AppStartupTests
    {
        public class MyDependency
        { }

        public class TestApp1 : AppCommand<TestArgs>
        {
            public static MyDependency Dependency { get; private set; } = null;

            public TestApp1(MyDependency testDep)
            {
                Dependency = testDep;
            }

            public static TestArgs Args { get; private set; } = null;

          

            public override void Execute(TestArgs options)
            {
                Args = options;
            }
        }

        [Theory, AutoData]
        public void Execute_ShouldStartTheApp_And_PassParsedArgs(int count, string subject)
        {
            AppStartup.Default().ExecuteApp<TestApp1, TestArgs>(new []{ "--count", count.ToString(), "--subject", subject});

            TestApp1.Args.Count.Should().Be(count);
            TestApp1.Args.Subject.Should().Be(subject);
            TestApp1.Dependency.Should().NotBeNull();
        }
    }
}