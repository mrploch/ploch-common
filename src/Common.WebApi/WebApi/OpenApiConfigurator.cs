using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Ploch.Common.WebApi;

public static class OpenApiConfigurator
{
    public static IServiceCollection ConfigureOpenApiOptions(this IServiceCollection services, OpenApiInfo apiDescription, string apiVersionString = "v1")
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
                               {
                                   string OperationIdSelector(ApiDescription description)
                                   {
                                       var name = $"{description.HttpMethod}_{description.HttpMethod}";

                                       name = name.ToPascalCase();

                                       if (name.StartsWith("GetGet", StringComparison.InvariantCultureIgnoreCase))
                                       {
                                           name = name[3..];
                                       }

                                       return name;
                                   }

                                   options.SwaggerGeneratorOptions.OperationIdSelector = OperationIdSelector;
                                   options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}{e.HttpMethod}");
                                   options.SwaggerDoc(apiVersionString, apiDescription);
                               });

        return services;
    }
}
