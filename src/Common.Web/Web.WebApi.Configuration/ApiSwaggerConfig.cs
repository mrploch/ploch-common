using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Ploch.Common.Web.WebApi.Configuration
{
    /// <summary>
    ///     Methods for configuring Swagger API documentation.
    /// </summary>
    public static class ApiSwaggerConfig
    {
        /// <summary>
        ///     Configures the OpenApi contract generator services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="name">The name of the api.</param>
        /// <param name="apiInfo">The API info.</param>
        public static void ConfigureOpenApiContractGeneratorServices(this IServiceCollection services, string name, OpenApiInfo apiInfo)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
                                   {
                                       setup.SwaggerDoc(name, apiInfo);
                                       setup.EnableAnnotations();
                                   });
        }

        /// <summary>
        ///     Configures the OpenApi contract generator for a <c>WebApplication</c>.
        /// </summary>
        /// <param name="app">The web app.</param>
        public static void ConfigureOpenApiContractGeneratorApp(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}