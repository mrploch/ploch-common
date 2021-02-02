using Ploch.Common.ConsoleApplication.Core;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class AppBuilderTests
    {
        [Fact]
        public void AppBuilder_should_not_require_any_configuration_to_work()
        {
            var builder = new AppBuilder();
            //builder.Bootstrapper().ExecuteApp<>();
        }
    }
}