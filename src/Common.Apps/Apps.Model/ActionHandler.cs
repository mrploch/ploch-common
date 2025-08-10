namespace Ploch.Common.Apps.Model;

/// <summary>
///     Base class for action handlers that operate on system applications.
///     This abstract class provides a generic implementation for handling actions related to system applications.
/// </summary>
/// <typeparam name="TSystemApplication">The type of system application being handled. Must implement <see cref="IActionTargetDescriptor" />.</typeparam>
/// <typeparam name="TActionInfo">The type of action information required for the action. Must implement <see cref="IActionInfo{TSystemApplication}" />.</typeparam>
public abstract class ActionHandler<TSystemApplication, TActionInfo> : ActionHandler<TSystemApplication, TActionInfo, ActionHandlerResult<TSystemApplication>>,
                                                                       IActionHandler<TSystemApplication, TActionInfo>
    where TSystemApplication : IActionTargetDescriptor where TActionInfo : IActionInfo<TSystemApplication>
{
    /// <summary>
    ///     Gets the priority of the action handler.  Handlers with higher priority are executed first.
    /// </summary>
    public abstract int Priority { get; }
}

/// <summary>
///     Base class for action handlers that operate on system applications and return a specific result type.
///     This abstract class provides a generic implementation for handling actions related to system applications,
///     allowing a specific result type to be returned.
/// </summary>
/// <typeparam name="TSystemApplication">The type of system application being handled. Must implement <see cref="IActionTargetDescriptor" />.</typeparam>
/// <typeparam name="TActionInfo">The type of action information required for the action. Must implement <see cref="IActionInfo{TSystemApplication}" />.</typeparam>
/// <typeparam name="TSystemApplicationActionResult">
///     The type of the action result. Must inherit from
///     <see cref="ActionHandlerResult{TSystemApplication}" />.
/// </typeparam>
public abstract class
    ActionHandler<TSystemApplication, TActionInfo, TSystemApplicationActionResult> : IActionHandler<TSystemApplication, TActionInfo,
    TSystemApplicationActionResult>
    where TActionInfo : IActionInfo<TSystemApplication>
    where TSystemApplicationActionResult : ActionHandlerResult<TSystemApplication>
    where TSystemApplication : IActionTargetDescriptor
{
    /// <summary>
    ///     Executes the action asynchronously.
    /// </summary>
    /// <param name="actionInfo">The action information required for execution.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.  The task result contains the action result.</returns>
    public abstract Task<TSystemApplicationActionResult> ExecuteAsync(TActionInfo actionInfo, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the execution ID for the action.
    /// </summary>
    /// <param name="actionInfo">The action information.</param>
    /// <returns>The execution ID for the action.</returns>
    protected ActionExecutionId<TSystemApplication> GetExecutionId(TActionInfo actionInfo) => new(actionInfo, GetType());
}
