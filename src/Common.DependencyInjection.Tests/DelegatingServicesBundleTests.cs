﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests
{
    public class DelegatingServicesBundleTests
    {
        [Theory]
        [AutoMockData]
        public void Configure_should_execute_on_service_collection_all_actions_stored_in_this_bundle(DelegatingServicesBundle sut, ServiceDescriptor[] descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                sut.Configure(collection => collection.Add(descriptor)).Should().BeSameAs(sut);
            }

            var serviceCollection = new ServiceCollection();
            sut.Configure(serviceCollection);

            serviceCollection.Should().BeEquivalentTo(descriptors);
        }
    }
}