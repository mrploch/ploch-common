namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassImplementingInterface : ITestInterface
{
    public const int DefaultInterfacePropertyValue = 42;

    public int InterfaceProperty { get; set; } = DefaultInterfacePropertyValue;
}
