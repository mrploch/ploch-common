using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Ploch.Common.Serialiation.NewtonsoftJson;
using Ploch.Common.Serialization;

namespace Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection;

/// <summary>
/// `IServiceCollection` extension methods for registering `NewtonsoftJson` serializer.
/// </summary>
public static class NewtonsoftJsonSerializerRegistration
{
    /// <summary>
    /// Registers <see cref="NewtonsoftJsonObjectSerializer"/> serializer as `<see cref="ISerializer"/> and <see cref="ISerializer{TSettings}"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="settings">Optional settings.</param>
    /// <returns>The same instance of the service collection that was passed to the method in the <paramref name="services"/> parameter.</returns>
    public static IServiceCollection AddNewtonsoftJsonSerializer(this IServiceCollection services, JsonSerializerSettings? settings = null)
    {
        var serializer = new NewtonsoftJsonObjectSerializer(settings);
        services.AddSingleton<ISerializer>(serializer);
        services.AddSingleton<ISerializer<JsonSerializerSettings>>(serializer);

        return services;
    }
}