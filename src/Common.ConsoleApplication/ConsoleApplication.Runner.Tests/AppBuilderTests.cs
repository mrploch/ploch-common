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
           // builder.Bootstrapper().
            //builder.Bootstrapper().ExecuteApp<>();
        }

        [Fact()]
        public void WithServicesTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void AddEventsTest()
        {
            Assert.True(false, "This test needs an implementation");
        }

        [Fact()]
        public void BootstrapperTest()
        {
            Assert.True(false, "This test needs an implementation");
        }
    }
}