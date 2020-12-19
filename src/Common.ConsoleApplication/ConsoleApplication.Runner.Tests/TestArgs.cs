using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests
{
    public class TestArgs
    {
        [Option] public string Subject { get; set; }

        [Option] public int Count { get; set; }
    }
}