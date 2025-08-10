namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents information about an action associated with a specific action target descriptor.
/// </summary>
/// <typeparam name="TActionTargetDescriptor">
///     The type of the action target descriptor. Must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
public class ActionInfo<TActionTargetDescriptor>(TActionTargetDescriptor systemApplication, string name)
    : IActionInfo<TActionTargetDescriptor> where TActionTargetDescriptor : IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the system application descriptor associated with the action information.
    ///     This property represents the specific implementation of <typeparamref name="TActionTargetDescriptor" />
    ///     tied to the action. It enables access to system-specific operations or configurations
    ///     required during runtime handling of actions.
    /// </summary>
    /// <value>
    ///     The descriptor of type <typeparamref name="TActionTargetDescriptor" />
    ///     that contains the details of the target system application linked to the action.
    /// </value>
    /// <typeparam name="TActionTargetDescriptor">
    ///     Defines the type of system application descriptor, ensuring it implements
    ///     the <see cref="IActionTargetDescriptor" /> interface.
    /// </typeparam>
    public TActionTargetDescriptor Descriptor => systemApplication;

    /// <summary>
    ///     Gets the name of the action associated with the current <see cref="IActionInfo" /> instance.
    ///     This property represents a human-readable identifier or label used to distinguish
    ///     the action in the context of its execution or configuration.
    /// </summary>
    /// <value>
    ///     A <see cref="string" /> containing the name of the action. This value is expected
    ///     to be non-null and provides a descriptive identifier for the associated action.
    /// </value>
    public string Name => name;
}
