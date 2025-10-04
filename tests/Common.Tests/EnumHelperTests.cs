namespace Ploch.Common.Tests;

public class EnumHelperTests
{
    [Fact]
    public void GetEnumEntries_should_return_all_enum_entries()
    {
        var dayOfWeeks = EnumHelper.GetEnumEntries<DayOfWeek>();
        dayOfWeeks.Should()
                  .HaveCount(Enum.GetValues<DayOfWeek>().Length)
                  .And.Contain([
                                       DayOfWeek.Monday,
                                       DayOfWeek.Tuesday,
                                       DayOfWeek.Wednesday,
                                       DayOfWeek.Thursday,
                                       DayOfWeek.Friday,
                                       DayOfWeek.Saturday,
                                       DayOfWeek.Sunday
                                   ]);
    }

    [Fact]
    public void GetFlags_should_return_all_set_flags()
    {
        var flags = (DayOfWeek.Monday | DayOfWeek.Wednesday | DayOfWeek.Friday).GetFlags();
        flags.Should().Contain([DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday]);
    }
}
