namespace Ploch.Common.Tests.TestTypes;

public class TestClass
{
    public TestClass(params string[] strings) => Strings = strings;

    public string this[int index]
    {
        get => Strings[index];
        set => Strings[index] = value;
    }

    public string[] Strings { get; set; }

    public int TestProperty { get; set; }
}
