using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Ploch.Common.Maui.Tests.TestAssembly1.ViewModels;
using Ploch.Common.Maui.Tests.TestAssembly1.Views;
using Ploch.Common.Maui.Tests.TestAssembly2;
using Ploch.Common.Maui.Tests.TestAssembly2.ViewModels;
using Ploch.Common.Maui.Tests.TestAssembly2.Views;
using Ploch.Common.Maui.ViewModels;
using Ploch.Common.Tests.TestAssembly1;

namespace Ploch.Common.Maui.Tests.ViewModels;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "This is test code and readability is more important than performance.")]
public class TypeDiscovererTests
{
    private static readonly IEnumerable<Type> TestAssembly1ViewTypes =
    [
        typeof(TestAssembly1ViewInheritingFromBaseContentPage1),
        typeof(TestAssembly1ViewInheritingFromIView1)
    ];

    private static readonly IEnumerable<Type> TestAssembly2ViewTypes =
    [
        typeof(TestAssembly2ViewInheritingFromIView1),
        typeof(TestAssembly2ViewInheritingFromBaseContentPage1)
    ];

    private static readonly IEnumerable<Type> LocalViewTypes =
        [typeof(LocalTestViewInheritingFromIView), typeof(LocalTestViewInheritingFromBaseContentPage), typeof(LocalTestViewInheritingFromBaseContentView)];

    private static readonly IEnumerable<Type> AllTestAssembliesViewTypes = TestAssembly1ViewTypes.Concat(TestAssembly2ViewTypes);

    private static readonly IEnumerable<Type> AllViewTypes = AllTestAssembliesViewTypes.Concat(LocalViewTypes);

    private static readonly IEnumerable<Type> TestAssembly1ViewModels =
        [typeof(TestAssembly1ViewModelInheritingFromIViewModel1), typeof(TestAssembly1ViewModelInheritingFromBaseViewModel1)];

    private static readonly IEnumerable<Type> TestAssembly2ViewModels =
        [typeof(TestAssembly2ViewModelInheritingFromIViewModel1), typeof(TestAssembly2ViewModelInheritingFromBaseViewModel1)];

    private static readonly IEnumerable<Type> LocalViewModels =
        [typeof(LocalTestViewModelInheritingFromIViewModel), typeof(LocalTestViewModelInheritingFromBaseViewModel)];

    private static readonly IEnumerable<Type> AllTestAssembliesViewModelTypes = TestAssembly1ViewModels.Concat(TestAssembly2ViewModels);

    private static readonly IEnumerable<Type> AllViewModelTypes = AllTestAssembliesViewModelTypes.Concat(LocalViewModels);

    private static readonly IEnumerable<Assembly?> TestAssemblies = new[] { TestAssembly1Info.Assembly, null, TestAssembly2Info.Assembly, null };

    [Fact]
    public void DiscoverViews_should_find_types_inheriting_from_IView_in_provided_assemblies_filtering_out_null_assemblies()
    {
        var viewTypes = TypeDiscoverer.DiscoverViews(TestAssemblies);

        ValidateDiscoveredTypes(viewTypes, AllTestAssembliesViewTypes);
    }

    [Fact]
    public void DiscoverViews_should_find_types_inheriting_from_IView_in_AppDomain_assemblies()
    {
        LoadTestAssembliesIntoAppDomain();

        var viewTypes = TypeDiscoverer.DiscoverViews();

        ValidateDiscoveredTypes(viewTypes, AllViewTypes);
    }

    [Fact]
    public void DiscoverViewModels_should_find_types_inheriting_from_IViewModel_in_provided_assemblies_filtering_out_null_assemblies()
    {
        var viewModelTypes = TypeDiscoverer.DiscoverViewModels(TestAssemblies);

        ValidateDiscoveredTypes(viewModelTypes, AllTestAssembliesViewModelTypes);
    }

    [Fact]
    public void DiscoverViewModels_should_find_types_inheriting_from_IViewModel_in_AppDomain_assemblies()
    {
        LoadTestAssembliesIntoAppDomain();

        var viewModelTypes = TypeDiscoverer.DiscoverViewModels();

        ValidateDiscoveredTypes(viewModelTypes, AllViewModelTypes);
    }

    private static void ValidateDiscoveredTypes(IEnumerable<Type> actual, IEnumerable<Type> expected)
    {
        actual.Should().HaveCount(expected.Count());
        actual.Should().Contain(expected);
    }

    private static void LoadTestAssembliesIntoAppDomain()
    {
        // Force loading of test assemblies into app domain
        _ = TestAssembly1Info.Assembly;
        _ = TestAssembly2Info.Assembly;
    }
}
