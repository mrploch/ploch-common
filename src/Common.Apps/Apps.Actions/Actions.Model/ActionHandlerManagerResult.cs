using Ploch.Common.ArgumentChecking;
using Ploch.Common.Results;

namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents the result of managing action handlers in the system.
/// </summary>
/// <typeparam name="TSystemApplication">
///     The type of the system application implementing the <see cref="IActionTargetDescriptor" /> interface.
///     This defines the context of the action execution.
/// </typeparam>
public class ActionHandlerManagerResult<TSystemApplication>(bool isSuccess,
                                                            ActionExecutionId<TSystemApplication> executionId,
                                                            IEnumerable<ErrorInfo>? errors,
                                                            IEnumerable<ActionHandlerResult<TSystemApplication>> handlerResults)
    : ActionHandlerResult<TSystemApplication>(isSuccess, executionId, errors) where TSystemApplication : IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the results of all individual action handlers that were executed during the action management process.
    /// </summary>
    /// <typeparam name="TSystemApplication">
    ///     The type of the system application implementing the <see cref="IActionTargetDescriptor" /> interface.
    /// </typeparam>
    /// <remarks>
    ///     The property contains a collection of <see cref="ActionHandlerResult{TSystemApplication}" /> objects.
    ///     Each element in the collection represents the result of a specific action handler execution,
    ///     providing details such as execution success, associated errors, and other context-defined information.
    /// </remarks>
    /// <value>
    ///     A collection of <see cref="ActionHandlerResult{TSystemApplication}" /> objects. This collection is guaranteed to be non-null.
    /// </value>
    public IEnumerable<ActionHandlerResult<TSystemApplication>> HandlerResults { get; } = handlerResults.NotNull();
}

/// <summary>
///     Provides factory methods to generate results for managing action handlers in a system.
/// </summary>
public static class ActionHandlerManagerResult
{
    /// <summary>
    ///     Creates a successful <see cref="ActionHandlerManagerResult{TSystemApplication}" /> instance.
    /// </summary>
    /// <typeparam name="TSystemApplication">The type of the system application, derived from <see cref="IActionTargetDescriptor" />.</typeparam>
    /// <param name="executionId">The identifier for the executed action, encapsulating details like the action information and handler type.</param>
    /// <param name="handlerResults">The results of the individual handlers invoked during the action execution.</param>
    /// <returns>A new instance of <see cref="ActionHandlerManagerResult{TSystemApplication}" /> marked as successful.</returns>
    public static ActionHandlerManagerResult<TSystemApplication> Success<TSystemApplication>(ActionExecutionId<TSystemApplication> executionId,
                                                                                             params IEnumerable<ActionHandlerResult<TSystemApplication>>
                                                                                                 handlerResults)
        where TSystemApplication : IActionTargetDescriptor => new(true, executionId, null, handlerResults);

    /// <summary>
    ///     Creates a failure <see cref="ActionHandlerManagerResult{TSystemApplication}" /> instance.
    /// </summary>
    /// <typeparam name="TSystemApplication">The type of the system application, derived from <see cref="IActionTargetDescriptor" />.</typeparam>
    /// <param name="executionId">The identifier for the executed action, encapsulating details such as the action information and handler type.</param>
    /// <param name="message">The error message describing the failure.</param>
    /// <param name="handlerResults">The results of the individual handlers invoked during the action execution.</param>
    /// <returns>A new instance of <see cref="ActionHandlerManagerResult{TSystemApplication}" /> marked as failed.</returns>
    public static ActionHandlerManagerResult<TSystemApplication> Failure<TSystemApplication>(ActionExecutionId<TSystemApplication> executionId,
                                                                                             string message,
                                                                                             params IEnumerable<ActionHandlerResult<TSystemApplication>>
                                                                                                 handlerResults)
        where TSystemApplication : IActionTargetDescriptor => new(false, executionId, [ ErrorInfo.Create(message) ], handlerResults);
}
