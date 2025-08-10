namespace Ploch.Common.Apps.Model;

/// <summary>
///     Defines a descriptor for action targets.
///     This interface provides a way to represent a specific target or system that actions can be associated with or executed upon.
/// </summary>
public interface IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the name of the action target.
    /// </summary>
    /// <remarks>
    ///     The <c>Name</c> property provides a human-readable identifier or designation
    ///     for the action target. This can be useful in scenarios where targets need to be
    ///     identifiable by unique or descriptive names in logs, user interfaces, or configurations.
    /// </remarks>
    string Name { get; }
}
