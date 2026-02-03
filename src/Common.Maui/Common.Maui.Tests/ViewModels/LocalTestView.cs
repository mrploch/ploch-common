using Ploch.Common.Maui.Views;
using Ploch.Lists.UI.MauiUI.ViewModels;

namespace Ploch.Common.Maui.Tests.ViewModels;

public class LocalTestViewInheritingFromIView : IView
{ }

public class LocalTestViewInheritingFromBaseContentView : BaseContentView
{ }

public class LocalTestViewInheritingFromBaseContentPage(IViewModel viewModel) : BaseContentPage(viewModel)
{ }
