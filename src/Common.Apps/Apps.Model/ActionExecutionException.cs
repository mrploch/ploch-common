namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents an exception that is thrown when a service action execution fails.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ActionExecutionException" /> class with the specified action type, application name, error message, and inner
///     exception.
/// </remarks>
/// <param name="actionType">The type of service action that failed.</param>
/// <param name="applicationName">The name of the application on which the action was performed.</param>
/// <param name="message">The message that describes the error.</param>
/// <param name="innerException">The exception that is the cause of the current exception.</param>
public class ActionExecutionException(IActionInfo actionInfo, string? message, Exception? innerException) : Exception(message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionExecutionException" /> class with the specified action type and application name.
    /// </summary>
    /// <param name="actionType">The type of service action that failed.</param>
    /// <param name="applicationName">The name of the application on which the action was performed.</param>
    public ActionExecutionException(IActionInfo actionInfo) : this(actionInfo, null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionExecutionException" /> class with the specified action type, application name, and error message.
    /// </summary>
    /// <param name="actionType">The type of service action that failed.</param>
    /// <param name="applicationName">The name of the application on which the action was performed.</param>
    /// <param name="message">The message that describes the error.</param>
    public ActionExecutionException(IActionInfo actionInfoe, string? message) : this(actionInfoe, message, null)
    { }

    /// <summary>
    ///     Gets the type of service action that failed.
    /// </summary>
    public IActionInfo ActionInfo { get; } = actionInfo;
}
