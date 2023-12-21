namespace Ploch.Common.WebApi.CrudController;

public interface ICommand
{
    CommandResult Execute(object input);
}

public interface ICommand<in TInput> : ICommand<TInput, CommandResult>
{ }

public interface ICommand<in TInput, out TOutput> : ICommand
    where TOutput : CommandResult
{
    TOutput Execute(TInput input);
}