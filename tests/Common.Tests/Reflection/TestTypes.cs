using JetBrains.Annotations;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

public class TestTypes
{
    // Classes should not be empty
#pragma warning disable S2094

// Code should not contain empty statements.
#pragma warning disable SA1106
    public class NoPropertiesObject;
#pragma warning restore SA1106

    public class SampleTestObject
    {
        public Guid Id { get; set; }

// Identifier contains a type name
#pragma warning disable CA1720
        public int Int { get; set; }
#pragma warning restore CA1720

        public string? TestString { get; set; }

        [UsedImplicitly]
        public DateTime TestDate { get; set; }

        [UsedImplicitly]
        public int? NullableInt { get; set; }

        public int? NullableIntSetToNull { get; set; }

        [UsedImplicitly]
        public SampleRecord? TestRecord { get; set; }

        public SampleRecord? TestRecordSetToNull { get; set; }

        [UsedImplicitly]
        public DateTime? NullableDateTime { get; set; }

        public DateTime? NullableDateTimeSetToNull { get; set; }

        public SampleSubType SubType { get; set; } = new();

        public TestStruct TestStruct { get; set; }

        public string this[int index] => $"Indexer {index}";

        public void UpdateSetToNullProperties()
        {
            NullableDateTimeSetToNull = null;
            NullableIntSetToNull = null;
            TestRecordSetToNull = null;
        }
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
