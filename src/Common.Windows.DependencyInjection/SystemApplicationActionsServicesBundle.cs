using System.Runtime.Versioning;
using Common.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.SystemApplications;
using Ploch.Common.Windows.SystemApplications.Operations;
using Ploch.Tools.SystemProfiles.Utilities.Windows.Managers.Services;

namespace Ploch.Common.Windows.DependencyInjection;

[SupportedOSPlatform("windows")]
public class SystemApplicationActionsServicesBundle : ConfigurableServicesBundle
{
    protected override void Configure(IConfiguration? configuration = null)
    {
        Services.AddConfigurationSection<PInvokeServiceStopperConfiguration>(Configuration.RequiredNotNull());
        Services.AddSingleton<ServiceStopStatusAwaiter>()
                .AddSingleton<IActionHandlerManager<IServiceDescriptor, SetStartModeOptionsActionInfo>,
                    ActionHandlerManager<IServiceDescriptor, SetStartModeOptionsActionInfo, IServiceStartModeUpdater>>()
                .AddSingleton<IActionHandlerManager<IServiceDescriptor, StopServiceActionInfo>,
                    ActionHandlerManager<IServiceDescriptor, StopServiceActionInfo, IServiceStopper>>();
        AddSystemApplicationActionHandler<IServiceDescriptor, SetStartModeOptionsActionInfo, IServiceStartModeUpdater, PInvokeServiceStartModeUpdater>();
        AddSystemApplicationActionHandler<IServiceDescriptor, SetStartModeOptionsActionInfo, IServiceStartModeUpdater, RegistryStartModeUpdater>();
        AddSystemApplicationActionHandler<IServiceDescriptor, StopServiceActionInfo, IServiceStopper, ServiceControllerServiceStopper>();
        AddSystemApplicationActionHandler<IServiceDescriptor, StopServiceActionInfo, IServiceStopper, PInvokeServiceStopper>();
        AddSystemApplicationActionHandler<IServiceDescriptor, StopServiceActionInfo, IServiceStopper, ServiceProcessKillerServiceStopper>();
    }

    private void AddSystemApplicationActionHandler<TSystemApplication, TActionInfo, THandlerInterface, THandler>()
        where TSystemApplication : ISystemApplicationDescriptor
        where TActionInfo : IActionInfo<TSystemApplication>
        where THandlerInterface : class, IActionHandler<TSystemApplication, TActionInfo>
        where THandler : class, IActionHandler<TSystemApplication, TActionInfo>, THandlerInterface
    {
        Services.AddSingleton<THandlerInterface, THandler>(); //.AddSingleton<IActionHandler<TSystemApplication, TActionInfo>, THandler>();
    }
}
