namespace Ploch.Common.ConsoleApplication.Core
{
    /// <summary>
    /// The command.
    /// </summary>
    /// <remarks>
    /// Represents an entire application or an application command.
    /// </remarks>
    /// <typeparam name="TOptions">Command arguments.</typeparam>
    public interface ICommand<in TOptions>
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <remarks>
        /// This method is called by the framework when command is selected
        /// for execution using command verb.
        /// If the application has only one command and does not use "verbs", the <c>Execute</c> method will
        /// always be called after arguments are parsed.
        /// </remarks>
        /// <param name="options">The parsed arguments.</param>
        void Execute(TOptions options);
    }

    /// <summary>
    /// The command with a string array as arguments.
    /// </summary>
    public interface ICommand : ICommand<string[]>
    { }
}