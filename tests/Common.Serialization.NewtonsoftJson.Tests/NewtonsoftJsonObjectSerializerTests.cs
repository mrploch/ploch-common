using System.Globalization;
using Newtonsoft.Json;
using Ploch.Common.Serialiation.NewtonsoftJson;
using Ploch.Common.Serialization.Tests;

namespace Ploch.Common.Serialization.NewtonsoftJson.Tests;

public class NewtonsoftJsonObjectSerializerTests : JsonSerializerWithSettingsTests<NewtonsoftJsonObjectSerializer, JsonSerializerSettings>
{
    protected override Action<JsonSerializerSettings> SettingsConfigurationAction => settings => { settings.Culture = CultureInfo.InvariantCulture; };

    [Fact]
    public void Serialize_should_use_options()
    {
        var defaultSerializer = new NewtonsoftJsonObjectSerializer();
        var serializer = new NewtonsoftJsonObjectSerializer(new() { Formatting = Formatting.Indented });

        serializer.Serialize(new { Foo = "Bar" }, settings => settings.Formatting.Should().Be(Formatting.Indented));
        defaultSerializer.Serialize(new { Foo = "Bar" }, settings => settings.Formatting.Should().Be(Formatting.None));
    }

    protected override NewtonsoftJsonObjectSerializer GetSerializer() => new();

    protected virtual JsonSerializerSettings CreateSettings() => new();
}
