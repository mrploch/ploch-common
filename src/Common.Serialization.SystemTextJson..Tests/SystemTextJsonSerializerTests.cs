﻿using System.Text.Json;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Serialization.Tests;
using Ploch.Common.Serialization.Tests.TestTypes;

namespace Ploch.Common.Serialization.SystemTextJson.Tests;

public class SystemTextJsonSerializerTests : JsonAsyncSerializerWithSettingsTests<SystemTextJsonSerializer, JsonSerializerOptions>
{
    [Fact]
    public void Serialize_should_use_options()
    {
        var defaultSerializer = new SystemTextJsonSerializer();

        var serializer = new SystemTextJsonSerializer(new JsonSerializerOptions { WriteIndented = true });

        serializer.Serialize(new { Foo = "Bar" }, settings => settings.WriteIndented.Should().Be(true));
        defaultSerializer.Serialize(new { Foo = "Bar" }, settings => settings.WriteIndented.Should().Be(false));
    }

    protected override SystemTextJsonSerializer GetSerializer()
    {
        return new SystemTextJsonSerializer();
    }

    protected virtual JsonSerializerOptions CreateSettings()
    {
        return new JsonSerializerOptions();
    }

    protected override Action<JsonSerializerOptions> SettingsConfigurationAction => options => options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
}