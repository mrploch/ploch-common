namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithProtectedSetter
{
    public static readonly string DefaultValue = "Default Value";

    public string PropertyWithProtectedSetter { get; protected set; } = DefaultValue;
}
