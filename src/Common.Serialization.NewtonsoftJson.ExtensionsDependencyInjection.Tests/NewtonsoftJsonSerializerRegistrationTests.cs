using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialiation.NewtonsoftJson;

namespace Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection.Tests;

public class NewtonsoftJsonSerializerRegistrationTests
{
    [Fact]
    public void AddNewtonsoftJsonSerializer_should_register_the_serializer()
    {
        var services = new ServiceCollection();
        services.AddNewtonsoftJsonSerializer(new JsonSerializerSettings { Formatting = Formatting.Indented });
        var provider = services.BuildServiceProvider();

        var serializer = provider.GetRequiredService<ISerializer>();
        serializer.Should().BeOfType<NewtonsoftJsonObjectSerializer>();
        var serializerWithSettings = provider.GetRequiredService<ISerializer<JsonSerializerSettings>>();
        serializerWithSettings.Should().BeOfType<NewtonsoftJsonObjectSerializer>().And.Be(serializer);

        var serialize = serializerWithSettings.Serialize(new { Foo = "Bar" }, settings => settings.Formatting.Should().Be(Formatting.Indented));

        var expected = JToken.Parse(@"{""Foo"": ""Bar""}");
        var actual = JToken.Parse(serialize);

        actual.Should().BeEquivalentTo(expected);
    }
}