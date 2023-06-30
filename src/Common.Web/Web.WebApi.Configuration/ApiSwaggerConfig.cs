using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Ploch.Common.Web.WebApi.Configuration
{
    // TODO: Either make it generic for the public or remove this class entirely
    public static class ApiSwaggerConfig
    {
        public static void ConfigureOpenApiContractGeneratorServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(setup =>
                                   {
                                       setup.SwaggerDoc("v1",
                                                        new OpenApiInfo
                                                        {
                                                            Title = "Ploch Lists Api",
                                                            Version = "1",
                                                            Description = "Ploch Lists Api",
                                                            Contact = new OpenApiContact
                                                                      {
                                                                          Email = "kris@ploch.dev", Name = "Kris Ploch", Url = new Uri("http://www.ploch.dev")
                                                                      }
                                                        });
                                       setup.EnableAnnotations();
                                   });
        }

        public static void ConfigureOpenApiContractGeneratorApp(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}