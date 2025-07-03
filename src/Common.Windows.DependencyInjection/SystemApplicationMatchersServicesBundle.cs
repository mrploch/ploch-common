using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.SystemApplications;
using Ploch.Tools.SystemProfiles.Core.Configuration;

namespace Ploch.Common.Windows.DependencyInjection;

/// <summary>
///     A services bundle that registers system application matchers for dependency injection.
/// </summary>
/// <remarks>
///     This bundle registers various matchers used to identify and categorize system applications
///     based on configured criteria.
/// </remarks>
public class SystemApplicationMatchersServicesBundle : IServicesBundle
{
    /// <summary>
    ///     Configures the service collection with system application matcher services.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <remarks>
    ///     Registers the following services:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><see cref="SystemApplicationMatcher" /> as a singleton</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="ServiceMatcher" /> as a singleton, configured with critical applications settings</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="ProcessMatcher" /> as a singleton, configured with critical applications settings</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<SystemApplicationMatcher>()
                .AddSingleton(provider => new ServiceMatcher(provider.GetRequiredService<IOptions<CriticalApplicationsConfiguration>>()))
                .AddSingleton(provider => new ProcessMatcher(provider.GetRequiredService<IOptions<CriticalApplicationsConfiguration>>()));
    }
}
