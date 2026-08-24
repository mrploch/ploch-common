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
