using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Windows.Wmi;

public static class WmiObjectQueryServiceRegistrations
{
    public static IServiceCollection AddWmiObjectQueryServices(this IServiceCollection services) =>
        services.AddSingleton<IWmiObjectQueryFactory, WmiObjectQueryFactory>().AddSingleton<IWmiConnectionFactory, DefaultWmiConnectionFactory>();
}
