using System.Diagnostics;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ploch.Common.ConsoleApplication.Runner.Configuration;
using Xunit;

namespace Ploch.Common.ConsoleApplication.Runner.Tests.Configuration
{
    public class DefaultConfigurationExtensionsTests
    {
        [Fact]
        public void UseDefaultConfiguration_without_parameters_should_load_json_and_env_config()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var currentDirectory = Directory.GetCurrentDirectory();
            var configuration = configurationBuilder.UseDefaultConfiguration(currentDirectory).Build();

            var configurationSection = configuration.GetSection("rootSection");

            var repoSection = configurationSection.GetSection("subSections");

            var configurationSections = repoSection.GetChildren();
            configurationSections.Should().HaveCount(2)
                                 .And.Contain(section => section.Key == "subSection1" && section.Value == "value1")
                                 .And.Contain(section => section.Key == "subSection2" && section.Value == "value2");
        }
    }
}