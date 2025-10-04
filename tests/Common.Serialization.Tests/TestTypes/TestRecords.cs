// ReSharper disable ClassNeverInstantiated.Global - these type is used in serialization

namespace Ploch.Common.Serialization.Tests.TestTypes;

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

public static class TestClasses
{
    public class TestComplexType(string ComplexTypeIntProp, string ComplexTypeDataType, object? ComplexTypeData, IEnumerable<string>? ComplexTypeStrings)
    {
        public string ComplexTypeIntProp { get; init; } = ComplexTypeIntProp;

        public string ComplexTypeDataType { get; init; } = ComplexTypeDataType;

        public object? ComplexTypeData { get; init; } = ComplexTypeData;

        public IEnumerable<string>? ComplexTypeStrings { get; init; } = ComplexTypeStrings;

        public void Deconstruct(out string ComplexTypeIntProp,
                                out string ComplexTypeDataType,
                                out object? ComplexTypeData,
                                out IEnumerable<string>? ComplexTypeStrings)
        {
            ComplexTypeIntProp = this.ComplexTypeIntProp;
            ComplexTypeDataType = this.ComplexTypeDataType;
            ComplexTypeData = this.ComplexTypeData;
            ComplexTypeStrings = this.ComplexTypeStrings;
        }
    }

    public class TestDataComplexType(int ComplexTypeIntProp,
                                     string ComplexTypeStringProp,
                                     TestEnum ComplexTypeEnumProp,
                                     IEnumerable<TestRecords.TestType2> ComplexTypeTestType2s)
    {
        public int ComplexTypeIntProp { get; init; } = ComplexTypeIntProp;

        public string ComplexTypeStringProp { get; init; } = ComplexTypeStringProp;

        public TestEnum ComplexTypeEnumProp { get; init; } = ComplexTypeEnumProp;

        public IEnumerable<TestRecords.TestType2> ComplexTypeTestType2s { get; init; } = ComplexTypeTestType2s;

        public void Deconstruct(out int ComplexTypeIntProp,
                                out string ComplexTypeStringProp,
                                out TestEnum ComplexTypeEnumProp,
                                out IEnumerable<TestRecords.TestType2> ComplexTypeTestType2s)
        {
            ComplexTypeIntProp = this.ComplexTypeIntProp;
            ComplexTypeStringProp = this.ComplexTypeStringProp;
            ComplexTypeEnumProp = this.ComplexTypeEnumProp;
            ComplexTypeTestType2s = this.ComplexTypeTestType2s;
        }
    }

    public class TestType2(string TestType2StrProperty, IEnumerable<TestRecords.TestType4>? TestType2TestType4s)
    {
        public string TestType2StrProperty { get; init; } = TestType2StrProperty;

        public IEnumerable<TestRecords.TestType4>? TestType2TestType4s { get; init; } = TestType2TestType4s;

        public void Deconstruct(out string TestType2StrProperty, out IEnumerable<TestRecords.TestType4>? TestType2TestType4s)
        {
            TestType2StrProperty = this.TestType2StrProperty;
            TestType2TestType4s = this.TestType2TestType4s;
        }
    }

    public class TestType3(string StrProperty, int IntProperty, TestEnum EnumProperty)
    {
        public string StrProperty { get; init; } = StrProperty;

        public int IntProperty { get; init; } = IntProperty;

        public TestEnum EnumProperty { get; init; } = EnumProperty;

        public void Deconstruct(out string StrProperty, out int IntProperty, out TestEnum EnumProperty)
        {
            StrProperty = this.StrProperty;
            IntProperty = this.IntProperty;
            EnumProperty = this.EnumProperty;
        }
    }

    public class TestType4(int TestType4IntProp, long TestType4LongProp)
    {
        public int TestType4IntProp { get; init; } = TestType4IntProp;

        public long TestType4LongProp { get; init; } = TestType4LongProp;

        public void Deconstruct(out int TestType4IntProp, out long TestType4LongProp)
        {
            TestType4IntProp = this.TestType4IntProp;
            TestType4LongProp = this.TestType4LongProp;
        }
    }
}

public static class TestRecords
{
    public record TestComplexTypeWithObjectProperty(string ComplexTypeIntProp,
                                                    string ComplexTypeDataType,
                                                    object? ComplexTypeData,
                                                    IEnumerable<string>? ComplexTypeStrings);

    public record TestDataComplexTypeWithEnumerableProperty(int ComplexTypeIntProp,
                                                            string ComplexTypeStringProp,
                                                            TestEnum ComplexTypeEnumProp,
                                                            IEnumerable<TestType2> ComplexTypeTestType2s);

    public record TestDataComplexType(int ComplexTypeIntProp, string ComplexTypeStringProp, TestEnum ComplexTypeEnumProp, TestType2 Type2Prop);

    public record TestType2(string TestType2StrProperty, IEnumerable<TestType4>? TestType2TestType4s);

    public record TestType3(string StrProperty, int IntProperty, TestEnum EnumProperty);

    public record TestType4(int TestType4IntProp, long TestType4LongProp);
}
