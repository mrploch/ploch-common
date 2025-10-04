using AutoFixture;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialization.Tests.TestTypes;
using Ploch.TestingSupport.XUnit3.AutoMoq;

namespace Ploch.Common.Serialization.Tests;

public abstract class JsonSerializerTests<TSerializer> where TSerializer : ISerializer
{
    protected const string SerializedTestType4 = @"{""TestType4IntProp"": 18, ""TestType4LongProp"": 140}";

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

    [Theory]
    [AutoMockData]
    public void Deserialize_NotGeneric_should_correctly_deserialize_data_(TestRecords.TestType4 testType)
    {
        var sut = GetSerializer();

        var deserializedObject = sut.Deserialize(SerializedTestType4, typeof(TestRecords.TestType4));
        var deserialized = deserializedObject.Should().BeOfType<TestRecords.TestType4>().Subject;

        ValidateDeserializedTestType4(deserialized);
    }

    [Fact]
    public void Convert_Generic_should_be_able_to_deserialize_complex_record_type_from_data()
    {
        var sut = GetSerializer();

        var (actualSerializedComplexType, expectedComplexType, expectedComplexTypeData) = GetSerializedComplexType(sut);

        var actualComplexType = sut.Deserialize<TestRecords.TestComplexTypeWithObjectProperty>(actualSerializedComplexType);

        actualComplexType!.ComplexTypeData.Should().NotBeNull();
        var deserializedData = sut.Convert<TestRecords.TestDataComplexTypeWithEnumerableProperty>(actualComplexType!.ComplexTypeData!);

        ValidateDeserializedComplexType(actualComplexType, expectedComplexType, deserializedData, expectedComplexTypeData);
    }

    [Theory]
    [AutoMockData]
    public void Convert_Generic_should_return_null_if_source_type_is_not_TDataJsonObject_and_not_the_target_type(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert<TestRecords.TestDataComplexType>(data.Type2Prop);

        deserializedData.Should().BeNull();
    }

    [Theory]
    [AutoMockData]
    public void Convert_Generic_should_return_null_if_source_object_is_null(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert<TestRecords.TestDataComplexType>(null);

        deserializedData.Should().BeNull();
    }

    [Theory]
    [AutoMockData]
    public void Convert_Generic_should_return_the_provided_value_if_target_type_is_the_same_type(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert<TestRecords.TestType2>(data.Type2Prop);

        deserializedData.Should().Be(data.Type2Prop);
    }

    [Theory]
    [AutoMockData]
    public void Convert_NotGeneric_should_return_null_if_source_type_is_not_TDataJsonObject_and_not_the_target_type(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert(typeof(TestRecords.TestDataComplexType), data.Type2Prop);

        deserializedData.Should().BeNull();
    }

    [Theory]
    [AutoMockData]
    public void Convert_NotGeneric_should_return_null_if_source_object_is_null(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert(typeof(TestRecords.TestDataComplexType), null);

        deserializedData.Should().BeNull();
    }

    [Theory]
    [AutoMockData]
    public void Convert_NotGeneric_should_return_the_provided_value_if_target_type_is_the_same_type(TestRecords.TestDataComplexType data)
    {
        var sut = GetSerializer();

        var deserializedData = sut.Convert(typeof(TestRecords.TestType2), data.Type2Prop);

        deserializedData.Should().Be(data.Type2Prop);
    }

    protected static (string, TestRecords.TestComplexTypeWithObjectProperty, TestRecords.TestDataComplexTypeWithEnumerableProperty) GetSerializedComplexType(
        ISerializer sut)
    {
        var fixture = new Fixture();
        var complexTypeData = fixture.Create<TestRecords.TestDataComplexTypeWithEnumerableProperty>();
        var wrapper = fixture.Build<TestRecords.TestComplexTypeWithObjectProperty>().With(type => type.ComplexTypeData, complexTypeData).Create();

        return (sut.Serialize(wrapper), wrapper, complexTypeData);
    }

    protected static void ValidateDeserializedTestType4(TestRecords.TestType4? deserialized)
    {
        deserialized.Should().NotBeNull();
        deserialized!.TestType4IntProp.Should().Be(18);
        deserialized.TestType4LongProp.Should().Be(140);
    }

    protected abstract TSerializer GetSerializer();

    private static void ValidateDeserializedComplexType(TestRecords.TestComplexTypeWithObjectProperty? actualComplexType,
                                                        TestRecords.TestComplexTypeWithObjectProperty expectedComplexType,
                                                        TestRecords.TestDataComplexTypeWithEnumerableProperty? actualComplexTypeData,
                                                        TestRecords.TestDataComplexTypeWithEnumerableProperty expectedComplexTypeData)
    {
        actualComplexType.Should().NotBeNull();
        actualComplexType!.ComplexTypeData.Should().NotBeNull();
        actualComplexType.Should().BeEquivalentTo(expectedComplexType, options => options.Excluding(type => type.ComplexTypeData));

        actualComplexTypeData.Should().BeEquivalentTo(expectedComplexTypeData);
    }
}
