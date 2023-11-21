// ReSharper disable ClassNeverInstantiated.Global - these type is used in serialization

namespace Ploch.Common.Serialization.Tests.TestTypes;

public static class TestRecords
{
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes - this is a test ype
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix    
    public enum TestEnum
#pragma warning restore CA1711
#pragma warning restore S2344
    {
        Entry1,
        Entry2,
        Entry3
    }

    public record TestComplexType(string ComplexTypeIntProp, string ComplexTypeDataType, object? ComplexTypeData, IEnumerable<string>? ComplexTypeStrings);

    public record TestDataComplexType(int ComplexTypeIntProp, string ComplexTypeStringProp, TestEnum ComplexTypeEnumProp, IEnumerable<TestType2> ComplexTypeTestType2s);

    public record TestType2(string TestType2StrProperty, IEnumerable<TestType4>? TestType2TestType4s);

    public record TestType3(string StrProperty, int IntProperty, TestEnum EnumProperty);

    public record TestType4(int TestType4IntProp, long TestType4LongProp);
}