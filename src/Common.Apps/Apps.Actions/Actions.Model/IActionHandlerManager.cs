namespace Ploch.Common.Apps.Model;

/// <summary>
///     Represents a manager interface for handling actions within a system application.
///     This interface extends the <see cref="IActionHandler{TActionTargetDescriptor, TActionInfo, TResult}" />
///     to provide specialized functionality for managing and executing multiple action handlers.
/// </summary>
/// <typeparam name="TSystemApplication">
///     The type of the system application, which must implement <see cref="IActionTargetDescriptor" />.
/// </typeparam>
/// <typeparam name="TActionInfo">
///     The type of the action information, which must implement <see cref="IActionInfo{TActionTargetDescriptor}" />.
/// </typeparam>
public interface
    IActionHandlerManager<TSystemApplication, in TActionInfo> : IActionHandler<TSystemApplication, TActionInfo, ActionHandlerManagerResult<TSystemApplication>>
    where TActionInfo : IActionInfo<TSystemApplication> where TSystemApplication : IActionTargetDescriptor
{ }
