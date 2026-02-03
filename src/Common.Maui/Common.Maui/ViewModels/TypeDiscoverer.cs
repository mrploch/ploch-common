using System.Reflection;
using Ploch.Common.Maui.Views;
using Ploch.Common.Reflection;
using Ploch.Lists.UI.MauiUI.ViewModels;

namespace Ploch.Common.Maui.ViewModels;

public static class TypeDiscoverer
{
    public static IEnumerable<Type> DiscoverViewModels() => DiscoverViewModels(AppDomain.CurrentDomain.GetAssemblies());

    public static IEnumerable<Type> DiscoverViewModels(params IEnumerable<Assembly?> assemblies) => FilterNullAssemblies(assemblies).GetImplementations<IViewModel>();

    public static IEnumerable<Type> DiscoverViews() => DiscoverViews(AppDomain.CurrentDomain.GetAssemblies());

    public static IEnumerable<Type> DiscoverViews(params IEnumerable<Assembly?> assemblies) => FilterNullAssemblies(assemblies).GetImplementations<IView>();

    private static IEnumerable<Assembly> FilterNullAssemblies(IEnumerable<Assembly?> assemblies) => assemblies.OfType<Assembly>();
}
