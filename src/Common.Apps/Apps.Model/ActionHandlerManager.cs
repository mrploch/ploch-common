using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Apps.Model;

/// <summary>
///     Manages the execution of multiple action handlers to process a given action
///     and ensures the coordination of their results.
/// </summary>
/// <typeparam name="TSystemApplication">
///     The type that represents the application or system on which the actions are executed.
///     Must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
/// <typeparam name="TActionInfo">
///     Information about the action being executed, which includes details about the application target.
///     Must implement <see cref="IActionInfo{TSystemApplication}" />.
/// </typeparam>
/// <typeparam name="TSystemActionHandler">
///     The type of the individual action handlers that will be invoked.
///     Must implement <see cref="IActionHandler{TSystemApplication, TActionInfo}" />.
/// </typeparam>
/// <remarks>
///     This class is responsible for iterating over a collection of action handlers and invoking their
///     <see cref="IActionHandler{TSystemApplication, TActionInfo, TResult}.ExecuteAsync" /> method. If a handler
///     succeeds in processing the action, it will immediately return the result.
///     If all handlers fail to process the action, a failure result is returned, consolidating all individual handler results.
/// </remarks>
public class ActionHandlerManager<TSystemApplication, TActionInfo, TSystemActionHandler>(IEnumerable<TSystemActionHandler> handlers)
    : ActionHandler<TSystemApplication, TActionInfo, ActionHandlerManagerResult<TSystemApplication>>, IActionHandlerManager<TSystemApplication, TActionInfo>
    where TActionInfo : IActionInfo<TSystemApplication>, IActionInfo<IActionTargetDescriptor>
    where TSystemActionHandler : IActionHandler<TSystemApplication, TActionInfo>
    where TSystemApplication : IActionTargetDescriptor
{
    private readonly IEnumerable<TSystemActionHandler> _handlers = handlers.OrderBy(h => h.Priority);

    /// <summary>
    ///     Executes the action asynchronously using a collection of system action handlers.
    /// </summary>
    /// <param name="actionInfo">
    ///     The action information of type <typeparamref name="TActionInfo" /> that describes the action
    ///     to be executed. Cannot be null.
    /// </param>
    /// <param name="cancellationToken">
    ///     Token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    ///     The result contains an <see cref="ActionHandlerManagerResult{TSystemApplication}" /> indicating
    ///     the execution outcome (success or failure) and any related results or errors from the handlers used.
    /// </returns>
    public override async Task<ActionHandlerManagerResult<TSystemApplication>> ExecuteAsync(TActionInfo actionInfo, CancellationToken cancellationToken)
    {
        actionInfo.NotNull();

        var results = new List<ActionHandlerResult<TSystemApplication>>();
        foreach (var actionHandler in _handlers)
        {
            var actionResult = await actionHandler.ExecuteAsync(actionInfo, cancellationToken);
            results.Add(actionResult);
            if (actionResult.IsSuccess)
            {
                return ActionHandlerManagerResult.Success(actionResult.ExecutionId, results);
            }
        }

        return ActionHandlerManagerResult.Failure(GetExecutionId(actionInfo), $"All handlers failed to execute {actionInfo}", results);
    }
}
