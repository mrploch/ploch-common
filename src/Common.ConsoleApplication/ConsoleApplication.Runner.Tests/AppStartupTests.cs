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

        public class TestApp : ICommand
        {
            public static MyDependency Dependency { get; private set; } = null;

            public TestApp(MyDependency testDep)
            {
                Dependency = testDep;
            }

            public static string[] Args { get; private set; } = null;

            /// <inheritdoc />
            public void Execute(string[] args)
            {
                Args = args;
            }
        }

        public class TestApp1 : ICommand<TestArgs>
        {
            public static MyDependency Dependency { get; private set; } = null;

            public TestApp1(MyDependency testDep)
            {
                Dependency = testDep;
            }

            public static TestArgs Args { get; private set; } = null;

          

            public void Execute(TestArgs options)
            {
                Args = options;
            }
        }

        [Theory, AutoData]
        public void Execute_ShouldStartTheAppProvided_AndPassArgs(string[] args)
        {
            AppStartup.ExecuteApp<TestApp>(args);
            TestApp.Args.Should().BeSameAs(args);
            TestApp.Dependency.Should().NotBeNull();

        }

        [Theory, AutoData]
        public void Execute_ShouldStartTheApp_And_PassParsedArgs(int count, string subject)
        {
            AppStartup.ExecuteApp<TestApp1, TestArgs>(new []{ "--count", count.ToString(), "--subject", subject});

            TestApp1.Args.Count.Should().Be(count);
            TestApp1.Args.Subject.Should().Be(subject);
            TestApp1.Dependency.Should().NotBeNull();
        }
    }
}