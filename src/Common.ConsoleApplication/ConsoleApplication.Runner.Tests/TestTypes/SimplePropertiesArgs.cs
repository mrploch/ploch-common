using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class SimplePropertiesArgs1
    {
        [Option('s')] public string StringProperty1 { get; set; }

        [Option('i')] public int IntProperty1 { get; set; }
    }

    public class SimplePropertiesArgs2
    {
        [Option('a')] public string AnotherStringProperty { get; set; }

        [Option('b')] public bool BoolProperty1 { get; set; } = false;

        [Option('s')] public string StringProperty1 { get; set; }
    }
}