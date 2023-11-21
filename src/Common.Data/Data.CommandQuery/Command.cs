namespace Ploch.Common.WebApi.CrudController;

public abstract class Command : ICommand
{
    public abstract CommandResult Execute(object input);
}

public abstract class Command<TInput, TOutput> : ICommand<TInput, TOutput> where TOutput : CommandResult
{
    public abstract TOutput Execute(TInput input);

    CommandResult ICommand.Execute(object input)
    {
        return Execute((TInput)input);
    }
}

public abstract class Command<TInput> : ICommand<TInput>
{
    public abstract CommandResult Execute(TInput input);

    CommandResult ICommand.Execute(object input)
    {
        return Execute((TInput)input);
    }
}