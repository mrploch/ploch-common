using AutoFixture;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Serialization.Tests.TestTypes;

namespace Ploch.Common.Serialization.Tests;

public abstract class JsonSerializerTests<TSerializer> where TSerializer : ISerializer
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
        var sut = GetSerializer();

        var deserialized = sut.Deserialize<TestRecords.TestType4>(SerializedTestType4);

        ValidateDeserializedTestType4(deserialized);
    }

    [Fact]
    public void Convert_Generic_should_be_able_to_deserialize_complex_record_type_from_data()
    {
        var sut = GetSerializer();

        var (actualSerializedComplexType, expectedComplexType, expectedComplexTypeData) = GetSerializedComplexType(sut);

        var actualComplexType = sut.Deserialize<TestRecords.TestComplexType>(actualSerializedComplexType);

        actualComplexType!.ComplexTypeData.Should().NotBeNull();
        var deserializedData = sut.Convert<TestRecords.TestDataComplexType>(actualComplexType!.ComplexTypeData!);

        ValidateDeserializedComplexType(actualComplexType, expectedComplexType, deserializedData, expectedComplexTypeData);
    }

    [Theory]
    [AutoMockData]
    public void Deserialize_NotGeneric_should_correctly_deserialize_data_(TestRecords.TestType4 testType)
    {

        var sut = GetSerializer();

        var deserializedObject = sut.Deserialize(SerializedTestType4, typeof(TestRecords.TestType4));
        var deserialized = deserializedObject.Should().BeOfType<TestRecords.TestType4>().Subject;
        
        ValidateDeserializedTestType4(deserialized);
    }

    private static void ValidateDeserializedComplexType(TestRecords.TestComplexType? actualComplexType,
                                                        TestRecords.TestComplexType expectedComplexType,
                                                        TestRecords.TestDataComplexType? actualComplexTypeData,
                                                        TestRecords.TestDataComplexType expectedComplexTypeData)
    {
        actualComplexType.Should().NotBeNull();
        actualComplexType!.ComplexTypeData.Should().NotBeNull();
        actualComplexType.Should().BeEquivalentTo(expectedComplexType, options => options.Excluding(type => type.ComplexTypeData));

        actualComplexTypeData.Should().BeEquivalentTo(expectedComplexTypeData);
    }

    protected static (string, TestRecords.TestComplexType, TestRecords.TestDataComplexType) GetSerializedComplexType(ISerializer sut /*, IEnumerable<string> strings*/)
    {
        var fixture = new Fixture();
        var complexTypeData = fixture.Create<TestRecords.TestDataComplexType>();
        var wrapper = fixture.Build<TestRecords.TestComplexType>().With(type => type.ComplexTypeData, complexTypeData).Create();

        return (sut.Serialize(wrapper), wrapper, complexTypeData);
    }
    
    protected const string SerializedTestType4 = @"{""TestType4IntProp"": 18, ""TestType4LongProp"": 140}";
    
    protected static void ValidateDeserializedTestType4(TestRecords.TestType4? deserialized)
    {
        deserialized.Should().NotBeNull();
        deserialized!.TestType4IntProp.Should().Be(18);
        deserialized.TestType4LongProp.Should().Be(140);
    }

    protected abstract TSerializer GetSerializer();
}