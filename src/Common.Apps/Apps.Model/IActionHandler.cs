namespace Ploch.Common.Apps.Model;

/// <summary>
///     Interface definition for handling actions with a structured implementation.
/// </summary>
/// <typeparam name="TActionTargetDescriptor">
///     The type representing the target descriptor for the action. It must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
/// <typeparam name="TActionInfo">
///     The type representing the information about the action being handled. It must implement <see cref="IActionInfo{TActionTargetDescriptor}" />.
/// </typeparam>
/// <typeparam name="TResult">
///     The result type produced after executing the action. It must extend <see cref="ActionHandlerResult{TActionTargetDescriptor}" />.
/// </typeparam>
public interface IActionHandler<TActionTargetDescriptor, in TActionInfo, TResult> where TActionInfo : IActionInfo<TActionTargetDescriptor>
                                                                                  where TResult : ActionHandlerResult<TActionTargetDescriptor>
                                                                                  where TActionTargetDescriptor : IActionTargetDescriptor
{
    /// <summary>
    ///     Represents the type of action information that an action handler is designed to process.
    /// </summary>
    /// <remarks>
    ///     The <c>ActionInfoType</c> property identifies the generic type parameter <c>TActionInfo</c> implemented in an
    ///     <see cref="IActionHandler{TActionTargetDescriptor, TActionInfo, TResult}" />.
    ///     This property is useful for determining the specific action information type at runtime, particularly in scenarios where the action handling
    ///     logic dynamically processes multiple action types.
    /// </remarks>
    /// <value>
    ///     A <see cref="Type" /> object representing the type of action information (<c>TActionInfo</c>) the handler is configured for.
    /// </value>
    /// <example>
    ///     This property can help validate or categorize supported action information types handled by a particular implementation of the
    ///     <see cref="IActionHandler{TActionTargetDescriptor, TActionInfo, TResult}" /> interface.
    /// </example>
    Type ActionInfoType => typeof(TActionInfo);

    /// <summary>
    ///     Executes the specified action asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TActionTargetDescriptor">The type of the action target descriptor.</typeparam>
    /// <typeparam name="TActionInfo">The type of the action information.</typeparam>
    /// <typeparam name="TResult">The type of the result returned by the action handler.</typeparam>
    /// <param name="actionInfo">The information describing the action to be executed.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="System.Threading.CancellationToken" /> instance that can be used to observe cancellation requests.
    ///     Default value is <see cref="System.Threading.CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the action execution.</returns>
    Task<TResult> ExecuteAsync(TActionInfo actionInfo, CancellationToken cancellationToken = default);
}

/// <summary>
///     Interface for handling actions within a specific application context, providing a mechanism
///     to execute actions and determine their priority in a structured manner.
/// </summary>
/// <typeparam name="TSystemApplication">
///     The type representing the target descriptor for the application or system associated
///     with this handler. It must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
/// <typeparam name="TActionInfo">
///     The type representing the details or metadata of the action being handled. It must implement
///     <see cref="IActionInfo{TSystemApplication}" />.
/// </typeparam>
public interface IActionHandler<TSystemApplication, in TActionInfo> : IActionHandler<TSystemApplication, TActionInfo, ActionHandlerResult<TSystemApplication>>
    where TActionInfo : IActionInfo<TSystemApplication> where TSystemApplication : IActionTargetDescriptor
{
    /// <summary>
    ///     Indicates the priority level of an action handler within the processing pipeline.
    /// </summary>
    /// <remarks>
    ///     The <c>Priority</c> property is used to determine the execution order of action handlers.
    ///     Action handlers with lower priority values are invoked earlier than those with higher values.
    ///     This property facilitates the orchestration of multiple action handlers, ensuring that critical or foundational
    ///     tasks are executed first based on their respective priorities.
    /// </remarks>
    /// <value>
    ///     An <see cref="int" /> representing the priority level of the action handler.
    ///     Smaller values indicate a higher priority, where <c>0</c> represents the highest priority level.
    /// </value>
    /// <example>
    ///     This property enables dynamic ordering and management of action handlers in a system that processes multiple handlers
    ///     for the same action. For example, it can be utilized within a collection of handlers to sort and invoke them based
    ///     on their priority values.
    /// </example>
    int Priority { get; }
}
