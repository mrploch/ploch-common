using ConsoleApplication.Simple;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Ploch.Common.CommandLineApps.Logging
{
    public static class AppBuilderLoggingExtensions
    {
        public static AppBuilder WithLogging(this AppBuilder appBuilder)
        {
            appBuilder.ConfigureServices(services => services.AddLogging());
            return appBuilder;
        }

        public static AppBuilder WithLogging(this AppBuilder appBuilder, Action<ILoggingBuilder> configure)
        { 
            appBuilder.ConfigureServices(services => services.AddLogging(configure));
            return appBuilder;
        }
    }
}