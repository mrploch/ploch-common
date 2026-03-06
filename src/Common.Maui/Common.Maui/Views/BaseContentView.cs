using AsyncAwaitBestPractices;
using Microsoft.Maui.Controls;
using Ploch.Lists.UI.MauiUI.ViewModels;

namespace Ploch.Common.Maui.Views;

public abstract class BaseContentView : ContentView, IView
{
    private bool _didAppear;
    private IViewModel? _viewModel;

    public virtual IViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            BindingContext = value;
            _viewModel = value;
        }
    }

    protected virtual void OnViewModelSet(IViewModel viewModel)
    { }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        Console.WriteLine(propertyName);

        var navigationPage = Application.Current.MainPage as NavigationPage;
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
        ViewModel.OnAppearingAsync().SafeFireAndForget();
    }

    protected virtual void OnViewDisappeared()
    { }
}
