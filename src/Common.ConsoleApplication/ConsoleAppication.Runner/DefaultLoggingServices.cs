using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class DefaultConfigurationServices : IServicesBundle
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("appsettings.json")
                                                          .AddEnvironmentVariables()
                                                          .Build();
        }
    }

    public class DefaultLoggingServices : IServicesBundle
    {
        /// <inheritdoc />
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(cfg => cfg.AddConsole());
        }
    }
}