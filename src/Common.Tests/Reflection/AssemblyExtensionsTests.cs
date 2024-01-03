using System.Reflection;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class AssemblyExtensionsTests
{
    [Fact]
    public void GetAssemblyDirectory_should_return_exact_location_of_the_dll()
    {
        var assemblyDirectory = Assembly.GetExecutingAssembly().GetAssemblyDirectory();
        assemblyDirectory.Should().NotBeNullOrWhiteSpace();
        var currentDirectory = Directory.GetCurrentDirectory();
        assemblyDirectory.Should().Be(currentDirectory);
    }
}