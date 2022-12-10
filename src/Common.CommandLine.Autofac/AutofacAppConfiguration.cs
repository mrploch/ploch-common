using Autofac;
using Autofac.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.CommandLine.Autofac
{
    public static class AutofacAppConfiguration
    {
        public static IConventionBuilder UseAutofac(this IConventionBuilder conventionBuilder, IContainer autofacContainer)
        {
            var serviceProvider = new AutofacServiceProvider(autofacContainer);

            return conventionBuilder.UseConstructorInjection(serviceProvider);
        }

        public static IConventionBuilder UseAutofac(this IConventionBuilder conventionBuilder,
                                                    ContainerBuilder containerBuilder,
                                                    IServiceCollection? serviceCollection = null)
        {
            if (serviceCollection != null)
            {
                containerBuilder.Populate(serviceCollection);
            }

            return conventionBuilder.UseAutofac(containerBuilder.Build());
        }

        public static TApp UseAutofac<TApp>(this TApp application, ContainerBuilder containerBuilder, IServiceCollection? serviceCollection = null)
            where TApp : CommandLineApplication
        {
            application.Conventions.UseAutofac(containerBuilder, serviceCollection);

            return application;
        }
    }
}