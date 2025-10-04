namespace Ploch.Common.Tests;

public class AssemblyInformationProviderTests
{
    private const string CurrentAssemblyProduct = "Ploch.Common";
    private const string CurrentAssemblyDescription = "Common utility libraries.";

    [Fact]
    public void GetAssemblyInformation_provides_info_for_the_object()
    {
        var assemblyInformation = this.GetAssemblyInformation();

        assemblyInformation.Product.Should().Be(CurrentAssemblyProduct);
        assemblyInformation.Description.Should().Be(CurrentAssemblyDescription);
        Version.TryParse(assemblyInformation.Version, out var version).Should().BeTrue();
        version.Should().BeEquivalentTo(typeof(AssemblyInformationProviderTests).Assembly.GetName().Version);
    }

    [Fact]
    public void GetAssemblyInformation_provides_info_for_the_type()
    {
        var assemblyInformation = GetType().GetAssemblyInformation();

        assemblyInformation.Product.Should().Be(CurrentAssemblyProduct);
        assemblyInformation.Description.Should().Be(CurrentAssemblyDescription);
        Version.TryParse(assemblyInformation.Version, out var version).Should().BeTrue();
        version.Should().BeEquivalentTo(typeof(AssemblyInformationProviderTests).Assembly.GetName().Version);
    }
}
