using System.IO.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.Wmi;

namespace Ploch.Common.Windows.SystemApplications;

public class SystemApplicationServicesBundle : IServicesBundle
{
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<ISystemApplicationsProvider, WmiSystemApplicationsProvider>()
                .AddServicesBundle<WmiObjectQueryServicesBundle>();
    }
}
