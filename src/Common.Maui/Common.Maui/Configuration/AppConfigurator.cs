using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Maui.ViewModels;

namespace Ploch.Common.Maui.Configuration;

public static class AppConfigurator
{
    public static IServiceCollection AddViewModels<TViewModelsAssemblyType>(this IServiceCollection services)
    {
        var viewModels = TypeDiscoverer.DiscoverViewModels(typeof(TViewModelsAssemblyType).Assembly, Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly());
        foreach (var viewModel in viewModels)
        {
            services.AddSingleton(viewModel);
        }

        return services;
    }

    public static IServiceCollection AddViews<TViewsAssemblyType>(this IServiceCollection services)
    {
        var views = TypeDiscoverer.DiscoverViews(typeof(TViewsAssemblyType).Assembly, Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly());

        foreach (var view in views)
        {
            services.AddSingleton(view);
        }

        return services;
    }
}
