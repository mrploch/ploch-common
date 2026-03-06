using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ploch.Common.DependencyInjection.Tests;

public class ConfigurableServiceBundleTests
{
    [Fact]
    public void AddServicesBundle_should_initialize_Configuration_property()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(new Dictionary<string, string?> { ["TestKey"] = "TestValue" })
                            .Build();
        var bundle = new TestConfigurableBundle();

        // Pre-assert
        bundle.Configuration.Should().BeNull();

        // Act
        services.AddServicesBundle(bundle, configuration);

        // Assert
        bundle.Configuration.Should().BeSameAs(configuration);
        bundle.ConfigureCalled.Should().BeTrue();
        bundle.ReceivedConfiguration.Should().BeSameAs(configuration);
    }

    // ... existing code ...
    [Fact]
    public void AddServicesBundle_should_throw_if_configuration_is_null()
    {
        // Arrange
        var services = new ServiceCollection();
        var bundle = new TestConfigurableBundle();

        // Act
        Action act = () => services.AddServicesBundle(bundle);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("*configuration*not initialized*");
    }

    private sealed class TestConfigurableBundle : ConfigurableServicesBundle
    {
        public bool ConfigureCalled { get; private set; }

        public IConfiguration? ReceivedConfiguration { get; private set; }

        protected override void Configure(IConfiguration configuration)
        {
            ConfigureCalled = true;
            ReceivedConfiguration = configuration;

            // ... existing code ...
        }
    }
}
