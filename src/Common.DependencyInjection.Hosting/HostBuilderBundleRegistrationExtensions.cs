using Microsoft.Extensions.Hosting;

namespace Ploch.Common.DependencyInjection.Hosting;

public static class HostBuilderBundleRegistrationExtensions
{
    public static IHostBuilder AddServicesBundle<TBundle>(this IHostBuilder hostBuilder)
        where TBundle : IServicesBundle, new() =>
        hostBuilder.ConfigureServices((context, services) => services.AddServicesBundle<TBundle>(context.Configuration));
}
