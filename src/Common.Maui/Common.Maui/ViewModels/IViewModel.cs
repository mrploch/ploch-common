namespace Ploch.Common.Maui.ViewModels;

/// <summary>
///     Contract for a view model bound to a view derived from <c>BaseContentPage</c> or <c>BaseContentView</c>.
/// </summary>
public interface IViewModel
{
    /// <summary>
    ///     Called when the associated view becomes visible.
    /// </summary>
    /// <returns>A task that completes once the appearance work has finished.</returns>
    Task OnAppearingAsync() => Task.CompletedTask;
}
