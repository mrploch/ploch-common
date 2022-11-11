using System.Diagnostics.CodeAnalysis;

namespace ConsoleApplication.Simple
{
    public interface ICommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary
        /// <remarks>
        /// OnExecuyt is called by the commandline app framework.
        /// This method should return an integer that represents the executed status code. As a starting point use:
        /// 0 - ExecutionSuccessful
        /// 10 - UnknownError
        /// Other codes/states are bespoke to nature of the functionality.
        /// </remarks>
        /// <returns>Executed status code integer</returns>
        [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Called dynamically by the CommandLineUtils library")]
        int OnExecute();
    }
}
