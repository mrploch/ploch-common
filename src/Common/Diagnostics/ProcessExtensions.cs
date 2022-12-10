namespace Ploch.Common.Diagnostics
{
    // TODO: This needs to be downgraded to support .NET Standard 2.0

    // public static class ProcessHelper
    // {
    //     public static Process Start( string fileName, Action<string> outputDataReceivedAction, params string[] arguments)
    //     {
    //         Guard.Argument(fileName, nameof(fileName)).NotNull().NotWhiteSpace();
    //         return Start(fileName, (_, args) => outputDataReceivedAction?.Invoke(args.Data), arguments);
    //     }
    //
    //
    //     public static Process Start( string fileName, DataReceivedEventHandler outputDataReceived, params string[] arguments)
    //     {
    //         Guard.Argument(fileName, nameof(fileName)).NotNull().NotWhiteSpace();
    //         var process = new Process { StartInfo = new ProcessStartInfo(fileName)};
    //         process.StartInfo.ArgumentList.AddMany(arguments);
    //         
    //         process.OutputDataReceived += outputDataReceived;
    //         process.Start();
    //         return process;
    //     }
    // }

    // public class ProcessBuilder
    // {
    //     private readonly Process _process;
    //
    //     public ProcessBuilder() : this(new Process())
    //     { }
    //
    //     public ProcessBuilder( Process process) 
    //     {
    //         _process = Guard.Argument(process, nameof(process)).NotNull();
    //     }
    //
    //     public ProcessBuilder WithFileName( string fileName)
    //     {
    //         _process.StartInfo.FileName = Guard.Argument(fileName, nameof(fileName)).NotNull().NotWhiteSpace();
    //         return this;
    //     }
    //
    //     public ProcessBuilder WithArguments( params string[] arguments)
    //     {
    //         Guard.Argument(arguments, nameof(arguments)).NotNull();
    //
    //         _process.StartInfo.ArgumentList.AddMany(arguments);
    //         return this;
    //     }
    //
    //     public ProcessBuilder WithArguments( IEnumerable<string> arguments)
    //     {
    //         Guard.Argument(arguments, nameof(arguments)).NotNull();
    //
    //         WithArguments((string[]) arguments);
    //         return this;
    //     }
    //
    //     public ProcessBuilder CaptureOutput( Action<string?> outputAction)
    //     {
    //         Guard.Argument(outputAction, nameof(outputAction)).NotNull();
    //
    //         _process.OutputDataReceived += (_, args) => outputAction(args.Data);
    //         return this;
    //     }
    // }
}