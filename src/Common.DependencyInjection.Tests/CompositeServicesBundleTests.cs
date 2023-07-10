using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.TestingSupport.Moq;
using SparkyTestHelpers.Moq;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests
{
    public class CompositeServicesBundleTests
    {
        [Theory]
        [AutoMockData]
        public void Configure_should_process_all_added_bundles(IServicesBundle[] constructorBundles, List<IServicesBundle> servicesBundles)
        {
            var sut = new CompositeServicesBundle(constructorBundles);

            foreach (var servicesBundle in servicesBundles)
            {
                sut.AddServices(servicesBundle).Should().BeSameAs(sut);
            }

            var serviceCollection = new ServiceCollection();
            sut.Configure(serviceCollection);

            servicesBundles.AddRange(constructorBundles);

            foreach (var servicesBundle in servicesBundles)
            {
                servicesBundle.Mock().Should().HaveOneCallTo(b => b.Configure(serviceCollection));
            }
        }
    }
}