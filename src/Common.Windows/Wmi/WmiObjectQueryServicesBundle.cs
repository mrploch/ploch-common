using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.Windows.Wmi;

public class WmiObjectQueryServicesBundle : IServicesBundle
{
    public void Configure(IServiceCollection services) =>
        services.AddSingleton<IWmiObjectQueryFactory, WmiObjectQueryFactory>().AddSingleton<IWmiConnectionFactory, DefaultWmiConnectionFactory>();
}
