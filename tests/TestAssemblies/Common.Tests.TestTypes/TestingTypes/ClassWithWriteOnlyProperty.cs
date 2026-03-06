namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithWriteOnlyProperty
{
    private string _writeOnlyProperty = string.Empty;

    public string WriteOnlyProperty
    {
        set => _writeOnlyProperty = value;
    }
}
