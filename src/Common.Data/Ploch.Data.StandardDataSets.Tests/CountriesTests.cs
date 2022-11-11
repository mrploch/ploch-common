using FluentAssertions;
using Ploch.Common.Data.StandardDataSets;
using Xunit;

namespace Ploch.Data.StandardDataSets.Tests
{
    public class CountriesTests
    {
        [Fact]
        public void GetRegions_should_return_all_regions()
        {
            var regionInfos = Regions.GetRegions();
            regionInfos.Count().Should().BeGreaterThan(500);
            regionInfos.Should().Contain(ri => ri.EnglishName == "Poland").And.Contain(ri => ri.EnglishName == "France");
        }

        [Fact]
        public void EnglishCountryNames_should_not_contain_duplicates()
        {
            var englishCountryNames = Regions.EnglishCountryNames();
            englishCountryNames.Count().Should().Be(englishCountryNames.Select(name => name.ToLower()).Distinct().Count());
        }
    }
}