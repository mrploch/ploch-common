// ReSharper disable ClassNeverInstantiated.Global - these type is used in serialization
namespace Ploch.Common.Serialization.Tests.TestTypes;

public static class TestRecords
{
#pragma warning disable S2344 // Enumeration type names should not have "Flags" or "Enum" suffixes - this is a test ype
    public enum TestEnum
#pragma warning restore S2344
    {
        Entry1,
        Entry2,
        Entry3
    }

    public record TestComplexType(string ClientId, string DataType, object? Data, IEnumerable<string>? TestType3s);
    
    public record TestDataComplexType(IEnumerable<TestType2> TestType2);

    public record TestType2(string TestType2StrProperty, IEnumerable<TestType4>? TestType4s);

    public record TestType3(string StrProperty, int IntProperty, TestEnum EnumProperty);

    public record TestType4(int TestType4IntProp, long TestType4LongProp);
}