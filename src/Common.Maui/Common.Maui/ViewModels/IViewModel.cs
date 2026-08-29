namespace Ploch.Common.Maui.ViewModels;

/// <summary>
///     Contract for a view model bound to a view derived from
///     <see cref="Ploch.Common.Maui.Views.BaseContentPage" /> or <see cref="Ploch.Common.Maui.Views.BaseContentView" />.
/// </summary>
public interface IViewModel
{
    /// <summary>
    ///     Called when the associated view becomes visible.
    /// </summary>
    /// <returns>A task that completes once the appearance work has finished.</returns>
    Task OnAppearingAsync() => Task.CompletedTask;
}
