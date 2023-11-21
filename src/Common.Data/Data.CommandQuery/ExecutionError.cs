namespace Ploch.Common.WebApi.CrudController;

public abstract record ExecutionError(string Message, Exception? Exception, IDictionary<string, object> AdditionalData);