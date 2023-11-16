using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Serialization.Tests.TestTypes;

namespace Ploch.Common.Serialization.SystemTextJson.Tests;

public abstract class JsonSerializerTests
{
    [Theory]
    [AutoMockData]
    public void Serialize_should_correctly_serialize_object(TestRecords.TestType4 testType)
    {
        var sut = GetSerializer();

        var serialized = sut.Serialize(testType);

        serialized.Should().NotBeNullOrEmpty();

        JToken.Parse(serialized).Should().BeEquivalentTo(JToken.FromObject(testType));
    }

    [Theory]
    [AutoMockData]
    public void Deserialize_should_correctly_deserialize_data()
    {
        var serialized = @"{""TestType4IntProp"": 18, ""TestType4LongProp"": 140}";

        var sut = GetSerializer();

        var deserialized = sut.Deserialize<TestRecords.TestType4>(serialized);

        deserialized.Should().NotBeNull();
        deserialized!.TestType4IntProp.Should().Be(18);
        deserialized.TestType4LongProp.Should().Be(140);
    }

    [Theory]
    [AutoMockData]
    public void Convert_Generic_should_be_able_to_deserialize_complex_record_type_from_data(TestRecords.TestDataComplexType complexTypeWithData,
                                                                                            IEnumerable<string> strings)
    {
        var sut = GetSerializer();

        var wrapper = new TestRecords.TestComplexType("test-1", "test-DataType", complexTypeWithData, strings);
        var serialized = sut.Serialize(wrapper);

        var deserialized = sut.Deserialize<TestRecords.TestComplexType>(serialized);

        deserialized.Should().NotBeNull();
        deserialized!.Data.Should().NotBeNull();

        var deserializedData = sut.Convert<TestRecords.TestDataComplexType>(deserialized.Data);

        deserializedData.Should().BeEquivalentTo(complexTypeWithData);
    }

    protected abstract ISerializer GetSerializer();
}