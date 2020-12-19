using CommandLine;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.TestTypes.VerbApps
{
    [Verb(Verb)]
    public class SimpleArgs2ForApp2 : SimplePropertiesArgs2
    {
        public const string Verb = "app2";
    }

    public class App2SimpleArgs : CommandRecordingExecute<SimpleArgs2ForApp2>
    { }
}