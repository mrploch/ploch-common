using Ploch.Common.Maui.ViewModels;
using Ploch.Common.Maui.Views;

namespace Ploch.Common.Maui.Tests.ViewModels;

public class LocalTestViewInheritingFromIView : IView
{ }

public class LocalTestViewInheritingFromBaseContentView : BaseContentView
{ }

public class LocalTestViewInheritingFromBaseContentPage(IViewModel viewModel) : BaseContentPage(viewModel)
{ }
