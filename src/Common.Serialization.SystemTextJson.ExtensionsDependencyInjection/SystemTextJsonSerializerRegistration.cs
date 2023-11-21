using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection;

/// <summary>
///     <c>SystemTextJsonSerializer</c> registration inside the <see cref="IServiceCollection" />.
/// </summary>
public static class SystemTextJsonSerializerRegistration
{
    /// <summary>
    ///     Registers <see cref="SystemTextJsonSerializer" /> serializer as `<see cref="ISerializer" />,
    ///     <see cref="ISerializer{TSettings}" />,
    ///     <see cref="IAsyncSerializer" /> and <see cref="IAsyncSerializer{TSettings}" />.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>
    ///     The same instance of the service collection that was passed to the method in the <paramref name="services" />
    ///     parameter.
    /// </returns>
    public static IServiceCollection AddSystemTextJsonSerializer(this IServiceCollection services)
    {
        var serializer = new SystemTextJsonSerializer();
        services.AddSingleton<ISerializer>(serializer);
        services.AddSingleton<ISerializer<JsonSerializerOptions>>(serializer);
        services.AddSingleton<IAsyncSerializer>(serializer);
        services.AddSingleton<IAsyncSerializer<JsonSerializerOptions>>(serializer);

        return services;
    }
}