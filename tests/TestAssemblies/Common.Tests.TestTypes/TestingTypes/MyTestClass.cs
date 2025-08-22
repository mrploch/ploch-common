using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

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
