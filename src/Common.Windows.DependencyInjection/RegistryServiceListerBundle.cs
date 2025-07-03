using System.IO.Abstractions;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;
using Ploch.Common.TypeConversion;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.Registry;
using Ploch.Common.Windows.SystemApplications.Operations;

namespace Ploch.Common.Windows.DependencyInjection;

[SupportedOSPlatform("windows")]
public class RegistryServiceListerBundle : IServicesBundle
{
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<IServicesLister, WindowsRegistryServicesLister>()
                .AddSingleton<IServiceStartModeUpdater, RegistryStartModeUpdater>()
                .AddSingleton<IInfResourceStringLoader, InfResourceStringLoader>()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<ITypeConverter<string, ValueType?>, EnumConverter>()
                .AddSingleton<CriticalProcessChecker>()
                .AddSingleton<CriticalServices>();
    }
}
