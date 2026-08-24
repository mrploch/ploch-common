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
                                   options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}{e.HttpMethod}");
                                   options.SwaggerDoc(apiVersionString, apiDescription);
                               });

        return services;
    }
}
