using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection;

/// <summary>
///     Represents a services bundle for registering and configuring the System.Text.Json-based serializer
///     within a dependency injection container.
/// </summary>
/// <remarks>
///     This class uses the <see cref="JsonSerializerOptions" /> provided during initialization to configure
///     the <see cref="SystemTextJsonSerializer" /> as the implementation for several serialization-related interfaces.
///     By default, if no options are provided, it uses the default settings from <see cref="JsonSerializerOptions.Default" />.
/// </remarks>
/// <param name="serializerOptions">JSON serializer options.</param>
public class SystemTextJsonSerializerServicesBundle(JsonSerializerOptions? serializerOptions = null) : IServicesBundle
{
    /// <summary>
    ///     Configures the service collection by registering the <see cref="SystemTextJsonSerializer" />
    ///     as various serializer interfaces and the configured <see cref="JsonSerializerOptions" />.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<SystemTextJsonSerializer>()
                .AddSingleton<ISerializer, SystemTextJsonSerializer>(provider => provider.GetRequiredService<SystemTextJsonSerializer>())
                .AddSingleton<ISerializer<JsonSerializerOptions>, SystemTextJsonSerializer>(provider => provider.GetRequiredService<SystemTextJsonSerializer>())
                .AddSingleton<IAsyncSerializer, SystemTextJsonSerializer>(provider => provider.GetRequiredService<SystemTextJsonSerializer>())
                .AddSingleton<IAsyncSerializer<JsonSerializerOptions>,
                    SystemTextJsonSerializer>(provider => provider.GetRequiredService<SystemTextJsonSerializer>())
                .AddSingleton(serializerOptions ?? JsonSerializerOptions.Default);
    }
}
