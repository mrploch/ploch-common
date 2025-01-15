using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class ByValueObjectComparerTests
{
    [Fact]
    public void Equals_should_return_true_if_two_objects_are_same_type_and_have_matching_property_values()
    {
        var guid = Guid.NewGuid();
        var dateTime = DateTime.Now.AddDays(3);
        var subGuid = Guid.NewGuid();
        var subDateTime = DateTime.Now.AddDays(4);
        var testType = CreateTestType(guid, dateTime, subGuid, subDateTime);
        var testType2 = CreateTestType(guid, dateTime, subGuid, subDateTime);
        var sut = new ByValueObjectComparer<SampleTestObject>();
        var equals = sut.Equals(testType, testType2);
        equals.Should().BeTrue();
        testType.Should().Be(testType2, sut);
    }

    private static SampleTestObject CreateTestType(Guid id, DateTime dateTime, Guid subId, DateTime subDateTime) =>
        new()
        {
            Id = id,
            Int = 42,
            TestString = "Test",
            TestDate = dateTime,
            SubType = new SampleSubType
                      {
                          SubGuid = subId,
                          SubInt = 24,
                          SubString = "SubTest",
                          SubDateTime = subDateTime
                      }
        };

    public class SampleTestObject
    {
        public Guid Id { get; set; }

        public int Int { get; set; }

        public string? TestString { get; set; }

        public DateTime TestDate { get; set; }

        public string this[int index] => $"Indexer {index}";

        public SampleSubType SubType { get; set; } = new();
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
