using FluentAssertions;
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ConfigureOpenApiOptions_should_throw_when_the_api_version_string_is_missing(string? apiVersionString)
    {
        var act = () => new ServiceCollection().ConfigureOpenApiOptions(ApiInfo, apiVersionString!);

        act.Should().Throw<ArgumentException>();
    }
}
