using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection.Tests;

public class SystemTextJsonSerializerRegistrationTests
{
    [Fact]
    public void AddSystemTextJsonSerializer_should_register_the_serializer()
    {
        var provider = BuildServiceProvider();

        var serializer = provider.GetRequiredService<ISerializer>();
        serializer.Should().BeOfType<SystemTextJsonSerializer>();
        var nonAsyncSerializer = provider.GetRequiredService<ISerializer<JsonSerializerOptions>>();
        provider.GetRequiredService<ISerializer<JsonSerializerOptions>>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(nonAsyncSerializer);

        var asyncSerializer = provider.GetRequiredService<IAsyncSerializer>();
        provider.GetRequiredService<IAsyncSerializer>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(asyncSerializer);

        provider.GetRequiredService<IAsyncSerializer<JsonSerializerOptions>>().Should().BeOfType<SystemTextJsonSerializer>().And.Be(asyncSerializer);
    }

    [Fact]
    public void SystemTextJsonSerializer_should_be_resolved_with_options_if_provided()
    {
        var provider = BuildServiceProvider(services => services.AddSingleton(new JsonSerializerOptions
                                                                              { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var serializer = provider.GetRequiredService<ISerializer>();

        var serializedObject = serializer.Serialize(new { MyPropertyName = "my property value" });

        serializedObject.Should().Contain("my_property_name");
    }

    private static IServiceProvider BuildServiceProvider(Func<IServiceCollection, IServiceCollection>? additionalRegistrations = null)
    {
        var services = new ServiceCollection();
        services.AddSystemTextJsonSerializer();
        additionalRegistrations?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
