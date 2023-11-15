using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Serialization.Tests.TestTypes;

namespace Ploch.Common.Serialization.SystemTextJson.Tests;

public class SystemTextJsonObjectConverterTests
{
    [Theory]
    [AutoMockData]
    public void Convert_Generic_should_be_able_to_deserialize_complex_record_type_from_data(TestRecords.TestDataComplexType complexTypeWithData,
                                                                                            IEnumerable<string> strings,
                                                                                            SystemTextJsonSerializer serializer,
                                                                                            SystemTextJsonObjectConverter sut)
    {
        var wrapper = new TestRecords.TestComplexType("test-1", "test-DataType", complexTypeWithData, strings);
        var serialized = serializer.Serialize(wrapper);

        var deserialized = serializer.Deserialize<TestRecords.TestComplexType>(serialized);

        var deserializedData = sut.Convert<TestRecords.TestDataComplexType>(deserialized.Data);

        deserializedData.Should().BeEquivalentTo(complexTypeWithData);
    }
}