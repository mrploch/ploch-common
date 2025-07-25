using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

public class ByValueObjectComparerTests
{
    [Theory]
    [AutoMockData]
    public void Equals_should_return_true_if_two_objects_are_same_type_and_have_matching_property_values(SampleTestObject testType1)
    {
        var testType2 = new SampleTestObject();

        testType1.CopyProperties(testType2);
        testType1.Should().BeEquivalentTo(testType2);

        // var guid = Guid.NewGuid();
        // var dateTime = DateTime.Now.AddDays(3);
        // var subGuid = Guid.NewGuid();
        // var subDateTime = DateTime.Now.AddDays(4);

        // var testType = CreateTestType(guid, dateTime, subGuid, subDateTime);
        // var testType2 = CreateTestType(guid, dateTime, subGuid, subDateTime);
        var sut = new ByValueObjectComparer<SampleTestObject>();
        var equals = sut.Equals(testType1, testType2);
        equals.Should().BeTrue();
        testType1.Should().Be(testType2, sut);
    }

    [Theory]
    [AutoMockData]
    public void Equals_should_return_false_if_two_objects_are_same_type_and_have_matching_property_values_except_one(SampleTestObject testType1)
    {
        var testType2 = new SampleTestObject();
        testType1.CopyProperties(testType2);
        testType2.SubType = new SampleSubType();
        testType2.SubType.SubGuid = Guid.NewGuid();
        testType2.SubType.SubDateTime = testType1.SubType.SubDateTime;
        testType2.SubType.SubInt = testType1.SubType.SubInt;
        testType2.SubType.SubString = testType1.SubType.SubString;
        testType2.TestStruct = new TestStruct(testType1.TestStruct.StructProperty, testType1.TestStruct.Struct2Property);

        testType2.SubType.SubGuid = Guid.NewGuid();

        // var testType = CreateTestType(guid, dateTime, subGuid, subDateTime);
        // var testType2 = CreateTestType(guid, dateTime, subGuid, subDateTime);
        var sut = new ByValueObjectComparer<SampleTestObject>();
        var equals = sut.Equals(testType1, testType2);
        equals.Should().BeFalse();
        testType1.Should().NotBe(testType2, sut);
    }

    private static SampleTestObject CreateTestType(Guid id, DateTime dateTime, Guid subId, DateTime subDateTime) =>
        new()
        { Id = id,
          Int = 42,
          TestString = "Test",
          TestDate = dateTime,
          SubType = new SampleSubType { SubGuid = subId, SubInt = 24, SubString = "SubTest", SubDateTime = subDateTime } };

    public class SampleTestObject
    {
        public Guid Id { get; set; }

        public int Int { get; set; }

        public string? TestString { get; set; }

        public DateTime TestDate { get; set; }

        public string this[int index] => $"Indexer {index}";

        public SampleSubType SubType { get; set; } = new();

        public TestStruct TestStruct { get; set; }
    }

    public record SampleRecord(int Id, string Name, DateTime CreatedDate);

    public class SampleSubType
    {
        public Guid SubGuid { get; set; }

        public int SubInt { get; set; }

        public string? SubString { get; set; }

        public DateTime SubDateTime { get; set; }
    }
}
