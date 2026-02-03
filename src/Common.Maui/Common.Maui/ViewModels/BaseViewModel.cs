using Ploch.Lists.UI.MauiUI.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ploch.Common.Maui.ViewModels;
public abstract class BaseViewModel : ObservableObject, IViewModel
{
    public virtual Task OnAppearingAsync() => Task.CompletedTask;
}
