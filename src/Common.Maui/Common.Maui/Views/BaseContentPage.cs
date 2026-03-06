using AsyncAwaitBestPractices;
using Microsoft.Maui.Controls;
using Ploch.Lists.UI.MauiUI.ViewModels;

namespace Ploch.Common.Maui.Views;

public abstract class BaseContentPage(IViewModel viewModel) : ContentPage, IView
{
    protected override void OnAppearing()
    {
        BindingContext = viewModel;

        viewModel.OnAppearingAsync().SafeFireAndForget();

        base.OnAppearing();
    }
}
