using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Ploch.Common.WebApi.Tests;

public class OpenApiConfiguratorTests
{
    private static OpenApiInfo ApiInfo => new() { Title = "Orders API", Version = "v1" };

    [Fact]
    public void ConfigureOpenApiOptions_should_return_the_same_service_collection_to_allow_chaining()
    {
        var services = new ServiceCollection();

        services.ConfigureOpenApiOptions(ApiInfo).Should().BeSameAs(services);
    }

    [Fact]
    public void ConfigureOpenApiOptions_should_register_the_swagger_generator()
    {
        var services = new ServiceCollection();

        services.ConfigureOpenApiOptions(ApiInfo);

        services.Should().Contain(descriptor => descriptor.ServiceType.FullName!.Contains("ISwaggerProvider", StringComparison.Ordinal));
    }

    [Fact]
    public void BuildOperationId_should_combine_the_controller_and_http_method_for_an_MVC_endpoint()
    {
        var apiDescription = new ApiDescription { HttpMethod = "GET", ActionDescriptor = new ActionDescriptor() };
        apiDescription.ActionDescriptor.RouteValues["controller"] = "Orders";

        OpenApiConfigurator.BuildOperationId(apiDescription).Should().Be("OrdersGET");
    }

    // Regression: this used the dictionary indexer, which throws KeyNotFoundException. Endpoints
    // registered outside MVC — Minimal API and FastEndpoints routes, the very styles this library
    // exists to support — carry no "controller" route value, so Swagger generation crashed.
    [Fact]
    public void BuildOperationId_should_fall_back_to_the_route_when_there_is_no_controller_route_value()
    {
        var apiDescription = new ApiDescription { HttpMethod = "POST", RelativePath = "orders/items", ActionDescriptor = new ActionDescriptor() };

        var act = () => OpenApiConfigurator.BuildOperationId(apiDescription);

        act.Should().NotThrow<KeyNotFoundException>();
        OpenApiConfigurator.BuildOperationId(apiDescription).Should().Be("POST_orders_items");
    }

    // Braces from a route template would otherwise land in the operationId, which breaks many
    // OpenAPI client generators.
    [Fact]
    public void BuildOperationId_should_strip_route_parameter_braces_from_the_fallback()
    {
        var apiDescription = new ApiDescription { HttpMethod = "GET", RelativePath = "orders/{id}/items", ActionDescriptor = new ActionDescriptor() };

        OpenApiConfigurator.BuildOperationId(apiDescription).Should().Be("GET_orders_id_items");
    }

    [Fact]
    public void BuildOperationId_should_fall_back_to_the_http_method_when_there_is_no_route_either()
    {
        var apiDescription = new ApiDescription { HttpMethod = "DELETE", ActionDescriptor = new ActionDescriptor() };

        OpenApiConfigurator.BuildOperationId(apiDescription).Should().Be("DELETE");
    }

    [Fact]
    public void ConfigureOpenApiOptions_should_throw_when_the_service_collection_is_null()
    {
        var act = () => ((IServiceCollection)null!).ConfigureOpenApiOptions(ApiInfo);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConfigureOpenApiOptions_should_throw_when_the_api_description_is_null()
    {
        var act = () => new ServiceCollection().ConfigureOpenApiOptions(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    // NotNullOrEmpty delegates to ArgumentException.ThrowIfNullOrEmpty, which distinguishes the two
    // cases. Asserting only ArgumentException would pass either way, since ArgumentNullException
    // derives from it, so each case is pinned separately to match the documented contract.
    [Fact]
    public void ConfigureOpenApiOptions_should_throw_ArgumentNullException_when_the_api_version_string_is_null()
    {
        var act = () => new ServiceCollection().ConfigureOpenApiOptions(ApiInfo, null!);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ConfigureOpenApiOptions_should_throw_ArgumentException_when_the_api_version_string_is_empty()
    {
        var act = () => new ServiceCollection().ConfigureOpenApiOptions(ApiInfo, string.Empty);

        // ThrowExactly, not Throw: ArgumentNullException derives from ArgumentException, so the
        // looser assertion would pass even if the empty case threw the null exception.
        act.Should().ThrowExactly<ArgumentException>();
    }
}
