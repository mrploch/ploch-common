using Ploch.Common.Results;

namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents the result of an action handler execution.
/// </summary>
/// <typeparam name="TActionTargetDescriptor">
///     The type that describes the target of the action. This type must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
public class ActionHandlerResult<TActionTargetDescriptor>(bool isSuccess,
                                                          ActionExecutionId<TActionTargetDescriptor> executionId,
                                                          IEnumerable<ErrorInfo>? errors) where TActionTargetDescriptor : IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the unique identifier for the execution of the action handler.
    /// </summary>
    /// <remarks>
    ///     The <see cref="ExecutionId" /> property represents an instance of the <see cref="ActionExecutionId{TActionTargetDescriptor}" /> class.
    ///     It encapsulates details about the action and the handler executed. This property is immutable and provides tracking
    ///     for the specific instance of the action's execution.
    /// </remarks>
    /// <typeparam name="TActionTargetDescriptor">
    ///     The type representing the action's target. This type must implement <see cref="IActionTargetDescriptor" />.
    /// </typeparam>
    /// <value>
    ///     An object of type <see cref="ActionExecutionId{TActionTargetDescriptor}" /> that uniquely identifies the execution of the action handler.
    /// </value>
    public ActionExecutionId<TActionTargetDescriptor> ExecutionId { get; } = executionId;

    /// <summary>
    ///     Gets a value indicating whether the action handler execution was successful.
    /// </summary>
    /// <remarks>
    ///     The <see cref="IsSuccess" /> property determines if the execution of the action handler
    ///     completed successfully without encountering any errors. A value of <c>true</c>
    ///     signifies success, while <c>false</c> indicates a failure or an issue during execution.
    ///     This property is particularly useful for downstream logic to determine the next course
    ///     of action or to handle any failures accordingly.
    /// </remarks>
    /// <value>
    ///     A <see cref="bool" /> value where <c>true</c> indicates successful execution and <c>false</c>
    ///     represents a failure.
    /// </value>
    public bool IsSuccess { get; } = isSuccess;

    /// <summary>
    ///     Gets the collection of errors encountered during the execution of the action handler.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Errors" /> property contains a collection of <see cref="ErrorInfo" /> instances
    ///     that provide detailed information about each error that occurred during the execution process.
    ///     If no errors were encountered, this property will be <c>null</c>.
    /// </remarks>
    /// <value>
    ///     A collection of <see cref="ErrorInfo" /> objects representing the errors encountered, or <c>null</c>
    ///     if no errors occurred.
    /// </value>
    public IEnumerable<ErrorInfo>? Errors { get; } = errors;
}

/// <summary>
///     Provides a mechanism to encapsulate the result of an action handler execution,
///     including information about success, associated errors, and the identifier for the execution context.
/// </summary>
public static class ActionHandlerResult
{
    /// <summary>
    ///     Creates an action handler result indicating a successful action execution.
    /// </summary>
    /// <typeparam name="TActionTargetDescriptor">
    ///     Specifies the type of the action target descriptor, representing the context or entity on which the action is executed.
    /// </typeparam>
    /// <param name="executionId">
    ///     The identifier for the execution context of the action, which contains information about the executed action and its handler.
    /// </param>
    /// <returns>
    ///     Returns an instance of <see cref="ActionHandlerResult{TActionTargetDescriptor}" /> representing a successful action execution,
    ///     with no associated errors.
    /// </returns>
    public static ActionHandlerResult<TActionTargetDescriptor> Success<TActionTargetDescriptor>(ActionExecutionId<TActionTargetDescriptor> executionId)
        where TActionTargetDescriptor : IActionTargetDescriptor => new(true, executionId, null);

    /// <summary>
    ///     Creates an instance of <see cref="ActionHandlerResult{TActionTargetDescriptor}" /> representing a failure
    ///     along with the provided <paramref name="executionId" /> and a collection of detailed error information.
    /// </summary>
    /// <typeparam name="TActionTargetDescriptor">The type of the action target descriptor that implements <see cref="IActionTargetDescriptor" />.</typeparam>
    /// <param name="executionId">The identifier associated with the execution of an action.</param>
    /// <param name="errors">An optional collection of <see cref="ErrorInfo" /> objects detailing the errors encountered.</param>
    /// <returns>
    ///     An <see cref="ActionHandlerResult{TActionTargetDescriptor}" /> indicating a failed action execution,
    ///     containing the provided <paramref name="executionId" /> and associated errors.
    /// </returns>
    public static ActionHandlerResult<TActionTargetDescriptor> Failure<TActionTargetDescriptor>(ActionExecutionId<TActionTargetDescriptor> executionId,
                                                                                                string message)
        where TActionTargetDescriptor : IActionTargetDescriptor => Failure(executionId, ErrorInfo.Create(message));

    /// <summary>
    ///     Creates an action handler result indicating a failed action execution.
    /// </summary>
    /// <typeparam name="TActionTargetDescriptor">
    ///     Specifies the type of the action target descriptor, representing the context or entity on which the action is executed.
    /// </typeparam>
    /// <param name="executionId">
    ///     The identifier for the execution context of the action, which contains information about the executed action and its handler.
    /// </param>
    /// <param name="exception">
    ///     The exception representing the details of the failure that occurred during the action execution.
    /// </param>
    /// <returns>
    ///     Returns an instance of <see cref="ActionHandlerResult{TActionTargetDescriptor}" /> representing a failed action execution,
    ///     with the associated exception wrapped in an <see cref="ErrorInfo" /> object.
    /// </returns>
    public static ActionHandlerResult<TActionTargetDescriptor>
        Failure<TActionTargetDescriptor>(ActionExecutionId<TActionTargetDescriptor> executionId, Exception exception)
        where TActionTargetDescriptor : IActionTargetDescriptor => Failure(executionId, ErrorInfo.Create(exception));

    /// <summary>
    ///     Creates an action handler result indicating a failed action execution.
    /// </summary>
    /// <typeparam name="TActionTargetDescriptor">
    ///     Specifies the type of the action target descriptor, representing the context or entity on which the action is executed.
    /// </typeparam>
    /// <param name="executionId">
    ///     The identifier for the execution context of the action, which contains information about the executed action and its handler.
    /// </param>
    /// <param name="errors">
    ///     A collection of error details that provides information about issues encountered during the action execution.
    /// </param>
    /// <returns>
    ///     Returns an instance of <see cref="ActionHandlerResult{TActionTargetDescriptor}" /> representing a failed action execution,
    ///     with the specified errors included.
    /// </returns>
    public static ActionHandlerResult<TActionTargetDescriptor> Failure<TActionTargetDescriptor>(ActionExecutionId<TActionTargetDescriptor> executionId,
                                                                                                params IEnumerable<ErrorInfo>? errors)
        where TActionTargetDescriptor : IActionTargetDescriptor => new(false, executionId, errors);
}
