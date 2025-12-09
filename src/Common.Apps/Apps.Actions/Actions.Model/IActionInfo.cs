namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents an interface for encapsulating action-related information,
///     including an associated target descriptor that provides metadata about the action's target.
/// </summary>
/// <typeparam name="TActionTargetDescriptor">
///     The type of the action target descriptor, which must implement <see cref="IActionTargetDescriptor" />.
///     The descriptor provides metadata or context about the target of the action.
/// </typeparam>
public interface IActionInfo<out TActionTargetDescriptor> : IActionInfo where TActionTargetDescriptor : IActionTargetDescriptor
{
    /// <summary>
    ///     Gets the descriptor providing metadata or context about the target of the action.
    /// </summary>
    /// <remarks>
    ///     The <c>Descriptor</c> property represents an instance of a type that provides information describing
    ///     the target associated with the action. This property is commonly used to retrieve detailed metadata
    ///     about the target, enabling specific functionality or access based on the target's attributes or context.
    /// </remarks>
    /// <value>
    ///     An instance of
    ///     <typeparamref name="TActionTargetDescriptor">
    ///         representing the target descriptor of the action.
    ///     </typeparamref>
    ///     This must be an object type that implements <see cref="IActionTargetDescriptor" />.
    /// </value>
    TActionTargetDescriptor Descriptor { get; }
}

/// <summary>
///     Represents the base interface for encapsulating essential information related to an action,
///     including its identifier or name. This foundational interface is extended by more specific
///     implementations to include additional metadata or descriptors for the action.
/// </summary>
public interface IActionInfo
{
    /// <summary>
    ///     Gets the name of the action.
    /// </summary>
    /// <remarks>
    ///     The <c>Name</c> property represents a textual identifier for the action. It is typically used to differentiate
    ///     between multiple actions or to provide a human-readable label for the action. This property ensures that each
    ///     action can be uniquely identified within the context of its usage.
    /// </remarks>
    /// <value>
    ///     A <see cref="string" /> representing the name of the action.
    ///     It serves as an identifier or label for the associated action.
    /// </value>
    string Name { get; }
}
