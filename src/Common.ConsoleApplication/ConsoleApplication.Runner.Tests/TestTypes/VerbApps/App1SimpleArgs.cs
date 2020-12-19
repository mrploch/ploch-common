using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes.VerbApps
{
    [Verb(Verb)]
    public class SimpleArgs1ForApp1 : SimplePropertiesArgs1
    {
        public const string Verb = "app1";
    }

    public class App1SimpleArgs : CommandRecordingExecute<SimpleArgs1ForApp1>
    { }
}