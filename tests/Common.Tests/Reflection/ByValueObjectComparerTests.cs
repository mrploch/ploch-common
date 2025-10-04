using Ploch.Common.Reflection;

namespace Ploch.Common.Tests.Reflection;

public class ByValueObjectComparerTests
{
    [Theory]
    [AutoMockData]
    public void Equals_should_return_true_if_two_objects_are_same_type_and_have_matching_property_values(TestTypes.SampleTestObject testType1)
    {
        testType1.UpdateSetToNullProperties();
        var testType2 = new TestTypes.SampleTestObject();

        testType1.CopyProperties(testType2);
        testType1.Should().BeEquivalentTo(testType2);

        var sut = new ByValueObjectComparer<TestTypes.SampleTestObject>();
        var equals = sut.Equals(testType1, testType2);

        equals.Should().BeTrue();
        testType1.Should().Be(testType2, sut);
    }

    [Theory]
    [AutoMockData]
    public void Equals_should_return_false_if_two_objects_are_same_type_and_have_matching_property_values_except_one(TestTypes.SampleTestObject testType1)
    {
        testType1.UpdateSetToNullProperties();
        var testType2 = new TestTypes.SampleTestObject();
        testType1.CopyProperties(testType2);
        testType2.SubType = new();
        testType2.SubType.SubGuid = Guid.NewGuid();
        testType2.SubType.SubDateTime = testType1.SubType.SubDateTime;
        testType2.SubType.SubInt = testType1.SubType.SubInt;
        testType2.SubType.SubString = testType1.SubType.SubString;
        testType2.TestStruct = new(testType1.TestStruct.StructProperty, testType1.TestStruct.Struct2Property);

        testType2.SubType.SubGuid = Guid.NewGuid();

        var sut = new ByValueObjectComparer<TestTypes.SampleTestObject>();
        var equals = sut.Equals(testType1, testType2);

        equals.Should().BeFalse();
        testType1.Should().NotBe(testType2, sut);
    }

    [Fact]
    public void GetHashCode_should_return_0_for_null_object()
    {
        var sut = new ByValueObjectComparer<TestTypes.SampleTestObject>();
        sut.GetHashCode(null!).Should().Be(0);
    }

    [Fact]
    public void GetHashCode_should_return_type_hash_code_for_object_with_no_properties()
    {
        var sut = new ByValueObjectComparer<TestTypes.NoPropertiesObject>();
        var obj = new TestTypes.NoPropertiesObject();
        sut.GetHashCode(obj).Should().Be(typeof(TestTypes.NoPropertiesObject).GetHashCode());
    }

    [Theory]
    [AutoMockData]
    public void GetHashCode_should_return_same_hash_code_for_equal_objects(TestTypes.SampleTestObject testType1)
    {
        testType1.UpdateSetToNullProperties();
        var testType2 = new TestTypes.SampleTestObject();
        testType1.CopyProperties(testType2);

        var sut = new ByValueObjectComparer<TestTypes.SampleTestObject>();
        var actualHashCode = sut.GetHashCode(testType1);
        actualHashCode.Should().Be(sut.GetHashCode(testType2));

        actualHashCode.Should().Be(ObjectHashCodeBuilder.GetHashCode(testType1));
    }
}
