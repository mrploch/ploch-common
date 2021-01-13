namespace Ploch.Common.ConsoleApplication.Core
{
    /// <summary>
    /// Represents the application text output.
    /// </summary>
    public interface IOutput
    {
        IOutput WriteLine<TContent>(TContent content, params object[] args);
        IOutput WriteLine();

        IOutput Write<TContent>(TContent content, params object[] args);

        IOutput WriteErrorLine<TContent>(TContent content, params object[] args);
        IOutput WriteErrorLine();

        IOutput WriteError<TContent>(TContent content, params object[] args);
    }
}