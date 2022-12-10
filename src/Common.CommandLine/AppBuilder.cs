using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.CommandLine
{
    public static class AppBuilder
    {
        public static CommandLineApplication CreateDefault()
        {
            var app = new CommandLineApplication();
            app.Conventions.UseDefaultConventions().UseCommandAttribute().UseCommandNameFromModelType();

            return app;
        }

        public static CommandLineApplication UseServiceProvider(this CommandLineApplication app, IServiceCollection serviceCollection)
        {
            app.Conventions.UseConstructorInjection(serviceCollection.BuildServiceProvider());

            return app;
        }
    }
}