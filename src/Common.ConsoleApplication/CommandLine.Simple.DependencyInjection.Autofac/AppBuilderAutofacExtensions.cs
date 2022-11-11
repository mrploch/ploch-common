using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using ConsoleApplication.Simple;

namespace Ploch.Common.CommandLine.Simple.DependencyInjection.Autofac
{
    public static class AppBuilderAutofacExtensions
    {
        public static AppBuilder UseAutofac(this AppBuilder appBuilder, Action<ContainerBuilder>? configurationAction = null)
        {
            return UseAutofac(appBuilder, ContainerBuildOptions.None, configurationAction);
        }

        public static AppBuilder UseAutofac(this AppBuilder appBuilder, ContainerBuildOptions buildOptions, Action<ContainerBuilder>? configurationAction = null)
        {
            appBuilder.WithServiceProviderFactory(new AutofacServiceProviderFactory(buildOptions, configurationAction));
            return appBuilder;
        } 
    }
}