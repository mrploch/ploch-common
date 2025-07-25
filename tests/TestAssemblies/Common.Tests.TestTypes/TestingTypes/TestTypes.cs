using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8603 // Possible null reference return.

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "NotAccessedField.Local")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public enum ByteEnum : byte
{
    Value1 = 1,
    Value2 = 2,
    Value3 = 3
}

public enum EnumWithCustomValues
{
    MinValue = -100,
    SecondValue = 100,
    MaxValue = 200
}

public enum EnumWithSpecialChars
{
    Special_Value_With_Characters1,
    Special_Value_With_Characters2,
    Special_Value_With_Characters
}

public enum LongEnum : long
{
    Value1 = 1L,
    Value2 = 2L,
    Value3 = 3L
}

public enum TestEnum
{
    FirstValue,
    SecondValue,
    ThirdValue
}

[Flags]
public enum TestEnumWithFlags
{
    None = 0,
    FirstValue = 1,
    SecondValue = 2,
    ThirdValue = 4,
    All = FirstValue | SecondValue | ThirdValue
}

public class TestTypeWithMixedSettersAndGetter
{
    public string? _stringPropNoGetter;

#pragma warning disable 649 //  [CS0649] Field 'TestTypes.TestTypeWithMixedSettersAndGetter._stringPropNoSetter' is never assigned to, and will always have its default value null
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public string? _stringPropNoSetter;
#pragma warning restore 649

    public int IntProp { get; set; }

    public string? StringProp { get; set; }

    public int IntProp2 { get; set; }

    public string? StringProp2 { get; set; }

    public string StringPropNoSetter => _stringPropNoSetter;

#pragma warning disable S2376 // Write-only properties should not be used
    public string StringPropNoGetter
#pragma warning restore S2376 // Write-only properties should not be used
    {
        set => _stringPropNoGetter = value;
    }
}

public class MyTestClass
{
    public int IntProp { get; set; }

    public int? NullableIntProp { get; set; }

    public string? StringProp { get; set; }

    public string? StringProp2 { get; set; }

    public TestTypeBase? TestTypeBaseProp { get; set; }

    public TestType2? TestType2Prop { get; set; }

    protected string? MyProtectedStringProp { get; set; }

    public DateTimeOffset DateTimeOffsetProp { get; set; }

    public DateTimeOffset? NullableDateTimeOffsetProp { get; set; }
}

public class TestTypeBase
{
    public string? BaseProperty { get; set; }
}

public class TestType2 : TestTypeBase
{ }

public class Class1
{
    public string? MyProperty { get; set; }
}

[Attribute2(nameof(Attribute2Attribute))]
public class ClassWith_Attribute2
{
    public string? MyProperty { get; set; }
}

[Attribute1_1(PropInt = 100, PropInt2 = 200)]
[Attribute2("Test2")]
public class ClassWith_Attribute1_1_And_Attribute2
{
    public string? MyProperty { get; set; }
}

[Attribute1("blah", PropInt = 111)]
public class ClassWithInherited_Attribute1_1_And_Attribute2 : ClassWith_Attribute1_1_And_Attribute2
{
    public string? MyProperty2 { get; set; }
}

public class Attribute1Attribute(string name) : Attribute
{
    public string Name { get; } = name;

    public int PropInt { get; set; }
}

public sealed class Attribute1_1Attribute : Attribute1Attribute
{
    /// <inheritdoc />
    public Attribute1_1Attribute() : base(nameof(Attribute1_1Attribute))
    { }

    public int PropInt2 { get; set; }
}

public class Attribute2Attribute(string name) : Attribute
{
    public string Name { get; } = name;

    public int PropInt { get; set; }

    public string Test() => "Test";
}

public class ClassWithPrivateSetter
{
    public static readonly string DefaultValue = Guid.NewGuid().ToString();

    public string PropertyWithPrivateSetter { get; private set; } = DefaultValue;
}

public class ClassWithProtectedSetter
{
    public static readonly string DefaultValue = "Default Value";

    public string PropertyWithProtectedSetter { get; protected set; } = DefaultValue;
}

public class ClassWithInternalSetter
{
    public static readonly string DefaultValue = "Default";

    public string PropertyWithInternalSetter { get; internal set; } = DefaultValue;
}

public class ClassWithIndexer
{
    private readonly Dictionary<int, string> _storage = new();

    public string this[int index]
    {
        get => _storage.TryGetValue(index, out var value) ? value : throw new ArgumentOutOfRangeException(nameof(index));
        set => _storage[index] = value;
    }
}

public class ClassWithMultiIndexer
{
    private readonly Dictionary<(int, int), string> _storage = new();

    public string this[int index, int key]
    {
        get => _storage.TryGetValue((index, key), out var value) ? value : throw new ArgumentOutOfRangeException(nameof(index));
        set => _storage[(index, key)] = value;
    }
}

public interface ITestInterface
{
    int InterfaceProperty { get; set; }
}

public class ClassImplementingInterface : ITestInterface
{
    public const int DefaultInterfacePropertyValue = 42;

    public int InterfaceProperty { get; set; } = DefaultInterfacePropertyValue;
}

public class ClassWithCustomGetter
{
    public const string CustomGetterValue = "Custom Getter Value";

    public string PropertyWithCustomGetter => CustomGetterValue;
}

public class ClassWithFieldsAndProperties
{
    public bool BoolField = true;

    public DateTime DateTimeField = DateTime.Now;

    public decimal DecimalField = 19.99m;

    public double DoubleField = 3.14;

    public Guid GuidField = Guid.NewGuid();
    public int IntField = 42;

    public string StringField = "Hello, World!";

    public bool BoolFieldProperty
    {
        get => BoolField;
        set => BoolField = value;
    }

    public DateTime DateTimeFieldProperty
    {
        get => DateTimeField;
        set => DateTimeField = value;
    }

    public decimal DecimalFieldProperty
    {
        get => DecimalField;
        set => DecimalField = value;
    }

    public double DoubleFieldProperty
    {
        get => DoubleField;
        set => DoubleField = value;
    }

    public Guid GuidFieldProperty
    {
        get => GuidField;
        set => GuidField = value;
    }

    public int IntFieldProperty
    {
        get => IntField;
        set => IntField = value;
    }

    public string StringFieldProperty
    {
        get => StringField;
        set => StringField = value;
    }
}

public class ClassWithWriteOnlyProperty
{
    private string _writeOnlyProperty = string.Empty;

    public string WriteOnlyProperty
    {
        set => _writeOnlyProperty = value;
    }
}

public class ClassWithValueAndReferenceTypes
{
    public int ValueTypeProperty { get; set; }

    public string? ReferenceTypeProperty { get; set; }
}

public class TestClassWithStaticProperties
{
    public static string? StaticStringProp { get; set; }

    public static int StaticIntProp { get; set; }

    public static int? StaticNullableIntProp { get; set; }
}

public struct TestStruct(int structProperty, TestStruct2 struct2Property)
{
    public int StructProperty { get; set; } = structProperty;

    public TestStruct2 Struct2Property { get; set; } = struct2Property;
}

public struct TestStruct2(int intProperty, string stringProperty)
{
    public int IntProperty { get; set; } = intProperty;

    public string StringProperty { get; set; } = stringProperty;
}

public class TestClassWithStaticFieldsAndProperties
{
    public const string PrivateStaticPropName = nameof(PrivateStaticProp);
    public const string ProtectedStaticPropName = nameof(ProtectedStaticProp);
    public const string InternalStaticPropName = nameof(InternalStaticProp);
    public const string PublicStaticPropName = nameof(PublicStaticProp);
    public static string PrivateStaticField = "Private Static Property";
    public static string ProtectedStaticField = "Protected Static Property";
    public static string InternalStaticField = "Internal Static Property";

    public static string PublicStaticField = "Public Static Property";

    public static string PublicStaticProp { get; set; } = PublicStaticField;

    private static string PrivateStaticProp { get; set; } = PrivateStaticField;

    private static string ProtectedStaticProp { get; set; } = ProtectedStaticField;

    internal static string InternalStaticProp { get; set; } = InternalStaticField;
}

public class ClassWithPrivateMembers
{
    public const string PrivateFieldName = nameof(_privateField);
    public static string PrivateFieldValue = Guid.NewGuid().ToString();
    private string _privateField = PrivateFieldValue;
}
