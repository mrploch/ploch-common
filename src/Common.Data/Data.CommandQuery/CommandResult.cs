using System.Collections.ObjectModel;

namespace Ploch.Common.WebApi.CrudController;

public record CommandResult(bool Success, IDictionary<string, object> Data, IEnumerable<ExecutionError> Errors)
{
    public CommandResult(bool success) : this(success, new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()), Array.Empty<ExecutionError>())
    { }

    public CommandResult(bool Success, IDictionary<string, object> Data) : this(Success, Data, Array.Empty<ExecutionError>())
    { }

    public void Deconstruct(out bool success)
    {
        Deconstruct(out success, out _);
    }

    public void Deconstruct(out bool success, out IDictionary<string, object> data)
    {
        Deconstruct(out success, out data, out _);
    }

    public void Deconstruct(out bool success, out IDictionary<string, object> data, out IEnumerable<ExecutionError> errors)
    {
        success = Success;
        data = Data;
        errors = Errors;
    }
}