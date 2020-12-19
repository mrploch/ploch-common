using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes
{
    public class ImmutableArgs1
    {
        [Option]
        public string Prop1Str { get; private set; }

        public ImmutableArgs1(string prop1Str)
        {
            Prop1Str = prop1Str;
        }
    }

    class ImmutableArgsApp1 : CommandRecordingExecute<ImmutableArgs1>
    {
        
    }
}