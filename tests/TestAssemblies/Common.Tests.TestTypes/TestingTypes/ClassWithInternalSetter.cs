namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithInternalSetter
{
    public static readonly string DefaultValue = "Default";

    public string PropertyWithInternalSetter { get; internal set; } = DefaultValue;
}
