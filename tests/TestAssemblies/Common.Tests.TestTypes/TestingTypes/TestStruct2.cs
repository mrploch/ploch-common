namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public struct TestStruct2(int intProperty, string stringProperty)
{
    public int IntProperty { get; set; } = intProperty;

    public string StringProperty { get; set; } = stringProperty;
}
