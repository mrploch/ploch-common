namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithCustomGetter
{
    public const string CustomGetterValue = "Custom Getter Value";

    public string PropertyWithCustomGetter => CustomGetterValue;
}
