namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public struct TestStruct(int structProperty, TestStruct2 struct2Property)
{
    public int StructProperty { get; set; } = structProperty;

    public TestStruct2 Struct2Property { get; set; } = struct2Property;
}
