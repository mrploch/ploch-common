using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.SystemApplications;

namespace Ploch.Common.Windows.Tests.SystemApplications;

public class WmiSystemApplicationsProviderTests
{
    [Fact]
    public void GetProcesses_should_retrieve_currently_runnig_processes_using_wmi()
    {
        var serviceProvider = new ServiceCollection().AddServicesBundle<SystemApplicationServicesBundle>().AddLogging().BuildServiceProvider();

        var systemApplicationsProvider = serviceProvider.GetRequiredService<ISystemApplicationsProvider>();

        var processes = systemApplicationsProvider.GetProcesses();

        processes.Should().NotBeNull().And.NotBeEmpty();
    }
}
