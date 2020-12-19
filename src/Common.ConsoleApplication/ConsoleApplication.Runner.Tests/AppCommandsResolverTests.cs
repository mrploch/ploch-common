using FluentAssertions;
using Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class AppCommandsResolverTests
    {
        [Fact]
        public void GetArgumentsType_should_return_type_of_IApp_generic_parameter_which_is_the_arguments_type()
        {
            var app1Args = AppCommandsResolver.GetArgumentsType(typeof(CommandWithEmptyArgs1));
            var app2Args = AppCommandsResolver.GetArgumentsType(typeof(CommandWithEmptyArgs2));

            app1Args.Should().Be<EmptyArgs1>();
            app2Args.Should().Be<EmptyArgs2>();
        }
    }
}