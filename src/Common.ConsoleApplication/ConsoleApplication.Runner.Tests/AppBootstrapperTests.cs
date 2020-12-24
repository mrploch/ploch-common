using FluentAssertions;
using Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes;
using Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes.VerbApps;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class AppBootstrapperTests
    {
        [Fact(DisplayName = "ExecuteApp should resolve argument types, choose correct app based on a verb and parse correct arguments")]
        public void ExecuteApps_resolves_argument_classes_and_executes_app_selected_by_verb_with_parsed_options()
        {
            var t2 = typeof(App2SimpleArgs);
            var bootstrapper = new AppBootstrapper();
            var apps = new[] {typeof(App1SimpleArgs), typeof(App2SimpleArgs)};
            var commandLine = "app2 -a val1 -b -s val2";
            
            bootstrapper.ExecuteApp(commandLine.Split(" "), apps);

            App1SimpleArgs.ExecuteCallCount.Should().Be(0);
            App1SimpleArgs.Args.Should().BeNull();

            App2SimpleArgs.ExecuteCallCount.Should().Be(1);
            var args = App2SimpleArgs.Args;
            args.AnotherStringProperty.Should().Be("val1");
            args.BoolProperty1.Should().BeTrue();
            args.StringProperty1.Should().Be("val2");
        }

        [Fact(Skip = "Immutable arguments don't seem to work.")]
        public void ExecuteApp_should_be_able_to_parse_args_with_immutable_properties()
        {
            var bootstrapper = new AppBootstrapper();
            var commandLine = "--Prop1Str str1";

            bootstrapper.ExecuteApp<ImmutableArgsApp1, ImmutableArgs1>(commandLine.Split(" "));

            ImmutableArgsApp1.Args.Should().NotBeNull();
            ImmutableArgsApp1.ExecuteCallCount.Should().Be(1);

        }

        [Fact]
        public void FactMethodName()
        {
            Assert.True(1 == 1);
        }
    }
}