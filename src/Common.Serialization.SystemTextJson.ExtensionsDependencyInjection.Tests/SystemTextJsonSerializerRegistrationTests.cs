using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection.Tests;

public class SystemTextJsonSerializerRegistrationTests
{
    [Fact]
    public void AddSystemTextJsonSerializer_should_register_the_serializer()
    {
        var services = new ServiceCollection();
        services.AddSystemTextJsonSerializer();
        var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<ISerializer>();
        serializer.Should().BeOfType<SystemTextJsonSerializer>();
        provider.GetRequiredService<ISerializer<JsonSerializerOptions>>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(serializer);
        provider.GetRequiredService<IAsyncSerializer>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(serializer);
        provider.GetRequiredService<IAsyncSerializer<JsonSerializerOptions>>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(serializer);
    }
}