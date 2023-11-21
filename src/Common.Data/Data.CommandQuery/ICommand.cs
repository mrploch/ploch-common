namespace Ploch.Common.WebApi.CrudController;

public interface ICommand
{
    CommandResult Execute(object input);
}

public interface ICommand<TInput> : ICommand<TInput, CommandResult>
{ }

public interface ICommand<TInput, TOutput> : ICommand where TOutput : CommandResult
{
    TOutput Execute(TInput input);
}