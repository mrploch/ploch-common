namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents a unique identifier for the execution of an action. It ties together the action information,
///     the handler processing the action, and metadata associated with the execution.
/// </summary>
/// <typeparam name="TSystemApplication">
///     The type that describes the target system or application in which the action will be executed. This type must implement
///     <see cref="IActionTargetDescriptor" />.
/// </typeparam>
public record ActionExecutionId<TSystemApplication>(IActionInfo<TSystemApplication> ActionInfo, Type HandlerType)
    where TSystemApplication : IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the additional metadata associated with the execution of an action.
    /// </summary>
    /// <remarks>
    ///     The <c>Metadata</c> property is a dictionary that allows associating arbitrary
    ///     key-value pairs with the execution of an action. This can be used to store
    ///     additional contextual information relevant to the action's execution, such as
    ///     execution time, custom tags, or processing details.
    ///     Keys are case-sensitive strings, while values can be any object, including null.
    /// </remarks>
    /// <value>
    ///     A dictionary where the keys are <see cref="string" /> and the values are <see cref="object" />.
    ///     By default, an empty dictionary is initialized when the object is created.
    /// </value>
    public IDictionary<string, object?> Metadata { get; } = new Dictionary<string, object?>();
}
