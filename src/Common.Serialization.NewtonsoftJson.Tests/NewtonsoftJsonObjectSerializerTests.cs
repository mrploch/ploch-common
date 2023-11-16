using Newtonsoft.Json;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Serialiation.NewtonsoftJson;
using Ploch.Common.Serialization.Tests.TestTypes;

namespace Ploch.Common.Serialization.SystemTextJson.Tests;

public class NewtonsoftJsonObjectSerializerTests : JsonSerializerTests
{
    [Fact]
    public void Serialize_should_use_options()
    {
        var defaultSerializer = new NewtonsoftJsonObjectSerializer();
        var serializer = new NewtonsoftJsonObjectSerializer(new JsonSerializerSettings() { Formatting = Formatting.Indented });

        serializer.Serialize(new { Foo = "Bar" }, settings => settings.Formatting.Should().Be(Formatting.Indented));
        defaultSerializer.Serialize(new { Foo = "Bar" }, settings => settings.Formatting.Should().Be(Formatting.None));
    }

    protected override ISerializer GetSerializer()
    {
        return new NewtonsoftJsonObjectSerializer();
    }
}