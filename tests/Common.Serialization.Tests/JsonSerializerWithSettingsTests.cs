using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialization.Tests.TestTypes;
using Ploch.TestingSupport.XUnit3.AutoMoq;

namespace Ploch.Common.Serialization.Tests;

public abstract class JsonSerializerWithSettingsTests<TSerializer, TSerializerSettings> : JsonSerializerTests<TSerializer>
    where TSerializer : ISerializer<TSerializerSettings>
{
    protected abstract Action<TSerializerSettings> SettingsConfigurationAction { get; }

    [Theory]
    [AutoMockData]
    public void Serialize_with_settings_should_correctly_serialize_object(TestRecords.TestType4 testType)
    {
        var sut = GetSerializer();

        var serialized = sut.Serialize(testType, SettingsConfigurationAction);

        serialized.Should().NotBeNullOrEmpty();

        JToken.Parse(serialized).Should().BeEquivalentTo(JToken.FromObject(testType));
    }

    [Fact]
    public void Deserialize_with_settings_should_correctly_deserialize_data()
    {
        var sut = GetSerializer();

        var deserialized = sut.Deserialize<TestRecords.TestType4>(SerializedTestType4, SettingsConfigurationAction);

        ValidateDeserializedTestType4(deserialized);
    }

    [Theory]
    [AutoMockData]
    public void Deserialize_NotGeneric_with_settings_should_correctly_deserialize_data_(TestRecords.TestType4 testType)
    {
        var sut = GetSerializer();

        var deserializedObject = sut.Deserialize(SerializedTestType4, typeof(TestRecords.TestType4), SettingsConfigurationAction);
        var deserialized = deserializedObject.Should().BeOfType<TestRecords.TestType4>().Subject;

        ValidateDeserializedTestType4(deserialized);
    }

    protected abstract override TSerializer GetSerializer();
}
