using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests;

public class ServicesBundleTests
{
    [Fact]
    public void Configure_should_run_Configure_method_on_all_dependencies()
    {
        var services = new ServiceCollection();

        services.AddServicesBundle<ServiceBundleA>();

        var provider = services.BuildServiceProvider();

        ValidateBundle<ServiceBundleA>(provider);
        ValidateBundle<ServiceBundleB>(provider);
        ValidateBundle<ServiceBundleC>(provider);
    }

    private static void ValidateBundle<TBundle>(IServiceProvider provider) where TBundle : TestServicesBundle
    {
        var bundle = provider.GetService<TBundle>();
        bundle.Should().NotBeNull();
        bundle.ConfigureCalled.Should().BeTrue();
    }

    public class TestServicesBundle : ServicesBundle
    {
        public bool ConfigureCalled { get; private set; }

        public override void DoConfigure()
        {
            Services.AddSingleton(GetType(), this);

            ConfigureCalled = true;
        }
    }

    public class ServiceBundleA : TestServicesBundle
    {
        protected override IEnumerable<IServicesBundle>? Dependencies => [ new ServiceBundleB(), new ServiceBundleC() ];
    }

    public class ServiceBundleB : TestServicesBundle;

    public class ServiceBundleC : TestServicesBundle;
}
