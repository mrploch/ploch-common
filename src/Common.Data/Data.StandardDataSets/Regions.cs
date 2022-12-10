using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ploch.Common.Data.StandardDataSets
{
    public static class Regions
    {
        public static IEnumerable<RegionInfo> GetRegions()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(ci => new RegionInfo(ci.Name));
        }

        public static IEnumerable<string> EnglishCountryNames()
        {
            return GetRegions().Select(region => region.EnglishName).Distinct().OrderBy(name => name);
        }
    }
}