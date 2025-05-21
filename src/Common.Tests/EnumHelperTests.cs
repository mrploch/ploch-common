using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests;

public class EnumHelperTests
{
    [Fact]
    public void GetEnumEntries_should_return_all_enum_entries()
    {
        var dayOfWeeks = EnumHelper.GetEnumEntries<DayOfWeek>();
        dayOfWeeks.Should()
                  .HaveCount(Enum.GetValues<DayOfWeek>().Length)
                  .And.Contain(new[]
                               {
                                   DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                               });
    }
}