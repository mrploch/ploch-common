
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.TestingSupport.Moq;
using Ploch.TestingSupport.Xunit.AutoFixture;
using SparkyTestHelpers.Moq;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests
{
    public class CompositeServicesBundleTests
    {
        class ServicesBundle1 : IServicesBundle
        {
            /// <inheritdoc />
            public void Configure(IServiceCollection serviceCollection)
            { }
        }
        [Theory, AutoMockData]
        public void Configure_should_process_all_added_bundles(IServicesBundle[] servicesBundles)
        {
            var sut = new CompositeServicesBundle();

            foreach (var servicesBundle in servicesBundles)
            {
                sut.AddServices(servicesBundle);
            }

            var serviceCollection = new ServiceCollection();
            sut.Configure(serviceCollection);

            foreach (var servicesBundle in servicesBundles)
            {
                servicesBundle.Mock().Should().HaveOneCallTo(b => b.Configure(serviceCollection));
            }
        }
    }
}