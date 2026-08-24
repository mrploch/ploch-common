using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.WebApi;

/// <summary>
///     Extension methods that register OpenAPI document generation for a Web API.
/// </summary>
public static class OpenApiConfigurator
{
    /// <summary>
    ///     Registers the built-in OpenAPI document provider together with Swagger generation for a single API version.
    /// </summary>
    /// <param name="services">The service collection to add the OpenAPI services to.</param>
    /// <param name="apiDescription">
    ///     The document metadata (title, version, description, contact, licence) published for the API.
    /// </param>
    /// <param name="apiVersionString">
    ///     The document name the generated Swagger document is registered under. Defaults to <c>v1</c>.
    /// </param>
    /// <returns>
    ///     The same <paramref name="services" /> instance, so further calls can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="services" />, <paramref name="apiDescription" /> or
    ///     <paramref name="apiVersionString" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="apiVersionString" /> is empty.
    /// </exception>
    /// <example>
    ///     <code>
    /// builder.Services.ConfigureOpenApiOptions(new OpenApiInfo
    ///                                          {
    ///                                              Title = "Orders API",
    ///                                              Version = "v1"
    ///                                          });
    ///     </code>
    /// </example>
    public static IServiceCollection ConfigureOpenApiOptions(this IServiceCollection services, OpenApiInfo apiDescription, string apiVersionString = "v1")
    {
        services.NotNull();
        apiDescription.NotNull();
        apiVersionString.NotNullOrEmpty();

        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
                               {
                                   options.CustomOperationIds(BuildOperationId);
                                   options.SwaggerDoc(apiVersionString, apiDescription);
                               });

        return services;
    }

    // ActionDescriptor.RouteValues has no "controller" entry for endpoints registered outside MVC —
    // Minimal API and FastEndpoints routes among them — and the dictionary indexer throws
    // KeyNotFoundException on a missing key, so Swagger generation would fail for exactly the
    // endpoint styles this library exists to support. Falls back to the route itself.
    internal static string BuildOperationId(ApiDescription apiDescription)
    {
        // ActionDescriptor is settable and not guaranteed populated for every ApiExplorer provider,
        // and this method exists precisely because the previous version assumed a shape it did not get.
        var routeValues = apiDescription.ActionDescriptor?.RouteValues;
        if (routeValues is not null && routeValues.TryGetValue("controller", out var controller) && !string.IsNullOrEmpty(controller))
        {
            return $"{controller}{apiDescription.HttpMethod}";
        }

        var relativePath = apiDescription.RelativePath;
        if (string.IsNullOrEmpty(relativePath))
        {
            return apiDescription.HttpMethod ?? string.Empty;
        }

        var (name, lossy) = NormaliseRoute(relativePath);

        // Normalisation is not injective: "orders/{id}" and "orders/id" both reduce to "orders_id",
        // and OpenAPI requires operationIds to be unique or client generation breaks. Only routes
        // that actually lost characters carry a disambiguator, so ordinary routes stay readable.
        return lossy
                   ? $"{apiDescription.HttpMethod}_{name}_{StableSuffix(relativePath)}"
                   : $"{apiDescription.HttpMethod}_{name}";
    }

    // Route templates carry characters an operationId should not: "orders/{id}" would become
    // "orders_{id}", and braces break many OpenAPI client generators. Separators collapse to single
    // underscores; anything else non-alphanumeric is dropped and reported through Lossy so the
    // caller can disambiguate.
    private static (string Name, bool Lossy) NormaliseRoute(string relativePath)
    {
        var builder = new StringBuilder(relativePath.Length);
        var lossy = false;

        foreach (var character in relativePath)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (character is '/' or '-' or '.')
            {
                if (builder.Length > 0 && builder[^1] != '_')
                {
                    builder.Append('_');
                }
            }
            else
            {
                lossy = true;
            }
        }

        return (builder.ToString().Trim('_'), lossy);
    }

    // Deterministic across processes and runs. string.GetHashCode is randomised per process, so a
    // generated document would differ between runs and break diffing of committed OpenAPI specs.
    private static string StableSuffix(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        return Convert.ToHexString(hash, 0, 4).ToLowerInvariant();
    }
}
