namespace Ploch.Common.ConsoleApplication.Core
{
    /*public interface ICommand
    {
        void Execute(string[] args);
    }*/

    public interface ICommand<in TOptions>
    {
        void Execute(TOptions options);
    }

    public interface ICommand : ICommand<string[]>
    { }
}