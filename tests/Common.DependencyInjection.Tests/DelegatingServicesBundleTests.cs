using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Ploch.TestingSupport.XUnit3.AutoMoq;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests;

public class DelegatingServicesBundleTests
{
    [Theory]
    [AutoMockData]
    public void Configure_should_execute_on_service_collection_all_actions_stored_in_this_bundle(DelegatingServicesBundle sut, ServiceDescriptor[] descriptors)
    {
        foreach (var descriptor in descriptors)
        {
            sut.Configure(tuple => tuple.services.Add(descriptor)).Should().BeSameAs(sut);
        }

        var serviceCollection = new ServiceCollection();
        sut.Configure(serviceCollection);

        serviceCollection.Should().BeEquivalentTo(descriptors);
    }
}
