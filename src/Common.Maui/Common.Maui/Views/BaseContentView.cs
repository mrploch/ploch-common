using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;
using Microsoft.Maui.Controls;
using Ploch.Lists.UI.MauiUI.ViewModels;

namespace Ploch.Common.Maui.Views;

/// <summary>
///     Base class for content views that are driven by an <see cref="IViewModel" />.
/// </summary>
/// <remarks>
///     Setting <see cref="ViewModel" /> also assigns it as the view's <c>BindingContext</c>. The view model's
///     <see cref="IViewModel.OnAppearingAsync" /> is invoked the first time the view renders.
/// </remarks>
public abstract class BaseContentView : ContentView, IView
{
    private bool _didAppear;
    private IViewModel? _viewModel;

    /// <summary>
    ///     Gets or sets the view model bound to this view. Assigning a value also sets the view's
    ///     <c>BindingContext</c>.
    /// </summary>
    public virtual IViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            var previous = _viewModel;

            BindingContext = value;
            _viewModel = value;

            // The view can render before a view model is assigned. OnViewAppeared has already
            // marked the appearance handled by then, and no further renderer change follows, so
            // without this the view model would never receive its first appearance notification.
            // Only a genuinely new model is notified: reassigning the same instance would
            // otherwise start its loading or subscription work twice within one appearance.
            if (value is not null && _didAppear && !ReferenceEquals(previous, value))
            {
                value.OnAppearingAsync().SafeFireAndForget();
            }
        }
    }

    /// <summary>
    ///     Extension point for derived classes to react to a view model being assigned.
    /// </summary>
    /// <remarks>
    ///     The base class does not currently invoke this method; overriding it alone has no effect.
    ///     A derived class that needs the callback must override <see cref="ViewModel" /> and call it.
    /// </remarks>
    /// <param name="viewModel">The view model that was assigned.</param>
    protected virtual void OnViewModelSet(IViewModel viewModel)
    { }

    /// <summary>
    ///     Tracks the view's render state so that <see cref="OnViewAppeared" /> and
    ///     <see cref="OnViewDisappeared" /> are raised once per appearance.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        Console.WriteLine(propertyName);

        // Application.MainPage is deprecated in .NET 9 MAUI. Windows is empty until the first window
        // is created, so the count is checked rather than indexing blindly; CA1826 rules out LINQ here.
        var windows = Application.Current?.Windows;
        var navigationPage = windows is { Count: > 0 } ? windows[0].Page as NavigationPage : null;
        if (propertyName == "Renderer" && IsVisible && !_didAppear)
        {
            _didAppear = true;
            OnViewAppeared();
        }
        else if (propertyName == "Renderer" && _didAppear && navigationPage != null)
        {
            OnViewDisappeared();
            _didAppear = false;
        }
    }

    /// <summary>
    ///     Method being called after ContentView appeared.
    /// </summary>
    protected virtual void OnViewAppeared()
    {
        BindingContext = ViewModel;
        ViewModel?.OnAppearingAsync().SafeFireAndForget();
    }

    /// <summary>
    ///     Called after the view has disappeared. The default implementation does nothing; override it to
    ///     release resources or cancel work started while the view was visible.
    /// </summary>
    protected virtual void OnViewDisappeared()
    { }
}
