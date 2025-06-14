using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection;

/// <summary>
///     A services bundle that registers <see cref="SystemTextJsonSerializer" /> and related services
///     in the dependency injection container.
/// </summary>
public class SystemTextJsonSerializerServicesBundle : IServicesBundle
{
    private readonly JsonSerializerOptions _serializerOptions = JsonSerializerOptions.Default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SystemTextJsonSerializerServicesBundle" /> class
    ///     with default <see cref="JsonSerializerOptions" />.
    /// </summary>
    public SystemTextJsonSerializerServicesBundle()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SystemTextJsonSerializerServicesBundle" /> class
    ///     with the specified <see cref="JsonSerializerOptions" />.
    /// </summary>
    /// <param name="serializerOptions">The JSON serializer options to use for serialization operations.</param>
    public SystemTextJsonSerializerServicesBundle(JsonSerializerOptions serializerOptions) => _serializerOptions = serializerOptions;

    /// <summary>
    ///     Configures the service collection by registering the <see cref="SystemTextJsonSerializer" />
    ///     as various serializer interfaces and the configured <see cref="JsonSerializerOptions" />.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public void Configure(IServiceCollection services)
    {
        services.AddSingleton<ISerializer, SystemTextJsonSerializer>()
                .AddSingleton<ISerializer<JsonSerializerOptions>, SystemTextJsonSerializer>()
                .AddSingleton<IAsyncSerializer, SystemTextJsonSerializer>()
                .AddSingleton<IAsyncSerializer<JsonSerializerOptions>, SystemTextJsonSerializer>()
                .AddSingleton(_serializerOptions ?? JsonSerializerOptions.Default);
    }
}
