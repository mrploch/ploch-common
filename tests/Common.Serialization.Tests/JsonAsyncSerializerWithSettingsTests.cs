using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialization.Tests.TestTypes;
using Ploch.TestingSupport.XUnit3.AutoMoq;

namespace Ploch.Common.Serialization.Tests;

public abstract class JsonAsyncSerializerWithSettingsTests<TSerializer, TSerializerSettings> : JsonSerializerWithSettingsTests<TSerializer, TSerializerSettings>
    where TSerializer : IAsyncSerializer<TSerializerSettings>
{
    [Theory]
    [AutoMockData]
    public async Task SerializeAsync_should_correctly_serialize_object(TestRecords.TestType4 testType)
    {
        await TestSerializeAsync(testType, (serializer, stream) => serializer.SerializeAsync(stream, testType));
    }

    [Theory]
    [AutoMockData]
    public async Task SerializeAsync_with_settings_should_correctly_serialize_object(TestRecords.TestType4 testType)
    {
        await TestSerializeAsync(testType, (serializer, stream) => serializer.SerializeAsync(stream, testType, SettingsConfigurationAction));
    }

    [Fact]
    public async Task DeserializeAsync_should_correctly_deserialize_data()
    {
        await TestDeserializeAsync((sut, stream, cancellationToken) => sut.DeserializeAsync<TestRecords.TestType4>(stream, cancellationToken));
    }

    [Fact]
    public async Task DeserializeAsync_with_settings_should_correctly_deserialize_data()
    {
        await TestDeserializeAsync((sut, stream, cancellationToken) =>
                                       sut.DeserializeAsync<TestRecords.TestType4>(stream, SettingsConfigurationAction, cancellationToken));
    }

    [Fact]
    public async Task DeserializeAsync_NonGeneric_with_settings_should_correctly_deserialize_data()
    {
        await TestDeserializeAsync(async (sut, stream, cancellationToken) =>
                                   {
                                       var deserializedObject =
                                           await sut.DeserializeAsync(stream, typeof(TestRecords.TestType4), SettingsConfigurationAction, cancellationToken);

                                       return deserializedObject.Should().BeOfType<TestRecords.TestType4>().Subject;
                                   });
    }

    [Fact]
    public async Task DeserializeAsync_NonGeneric_should_correctly_deserialize_data()
    {
        await TestDeserializeAsync(async (sut, stream, cancellationToken) =>
                                   {
                                       var deserializedObject = await sut.DeserializeAsync(stream, typeof(TestRecords.TestType4), cancellationToken);

                                       return deserializedObject.Should().BeOfType<TestRecords.TestType4>().Subject;
                                   });
    }

    protected abstract override TSerializer GetSerializer();

    private async Task TestSerializeAsync(TestRecords.TestType4 testType, Func<TSerializer, Stream, Task> serializeAction)
    {
        var sut = GetSerializer();

        using var stream = new MemoryStream();

        await serializeAction(sut, stream);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var serialized = await reader.ReadToEndAsync();

        serialized.Should().NotBeNullOrEmpty();

        JToken.Parse(serialized).Should().BeEquivalentTo(JToken.FromObject(testType));
    }

    private async Task TestDeserializeAsync(Func<TSerializer, Stream, CancellationToken, ValueTask<TestRecords.TestType4?>> deserializeAction)
    {
        var sut = GetSerializer();

        using var stream = await GetSerializedTestType4Stream();
        var deserialized = await deserializeAction(sut, stream, CancellationToken.None);

        ValidateDeserializedTestType4(deserialized);
    }

    private static async Task<MemoryStream> GetSerializedTestType4Stream()
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(SerializedTestType4);
        await writer.FlushAsync();
        stream.Position = 0;

        return stream;
    }
}
