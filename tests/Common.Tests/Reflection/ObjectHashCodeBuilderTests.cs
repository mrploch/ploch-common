using Ploch.Common.Reflection;

namespace Ploch.Common.Tests.Reflection;

public class ObjectHashCodeBuilderTests
{
    [Fact]
    public void GetHashCode_should_return_0_for_null_object()
    {
        ObjectHashCodeBuilder.GetHashCode(null!).Should().Be(0);
    }

    [Fact]
    public void GetHashCode_should_return_type_hash_code_for_object_with_no_properties()
    {
        var sut = new TestTypes.NoPropertiesObject();
        var obj = new TestTypes.NoPropertiesObject();
        ObjectHashCodeBuilder.GetHashCode(obj).Should().Be(typeof(TestTypes.NoPropertiesObject).GetHashCode());
    }

    [Theory]
    [AutoMockData]
    public void GetHashCode_should_return_same_hash_code_for_equal_objects(TestTypes.SampleTestObject testType1)
    {
        testType1.UpdateSetToNullProperties();
        var testType2 = new TestTypes.SampleTestObject();
        testType1.CopyProperties(testType2);

        ObjectHashCodeBuilder.GetHashCode(testType1).Should().Be(ObjectHashCodeBuilder.GetHashCode(testType2));
    }

    [Theory]
    [AutoMockData]
    public void GetHashCode_should_return_different_hash_codes_for_unequal_objects(TestTypes.SampleTestObject testType1)
    {
        testType1.UpdateSetToNullProperties();
        var testType2 = new TestTypes.SampleTestObject();
        testType1.CopyProperties(testType2);
        testType2.Id = Guid.NewGuid(); // Make them different

        var sut = new ByValueObjectComparer<TestTypes.SampleTestObject>();

        sut.GetHashCode(testType1).Should().NotBe(sut.GetHashCode(testType2));
    }
}
