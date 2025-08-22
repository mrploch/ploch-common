namespace Ploch.Common.Tests.TestTypes.TestingTypes;

[Attribute1("blah", PropInt = 111)]
public class ClassWithInherited_Attribute1_1_And_Attribute2 : ClassWith_Attribute1_1_And_Attribute2
{
    public string? MyProperty2 { get; set; }
}
