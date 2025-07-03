using System;

namespace Ploch.Common.Randomizers;

public class DateTimeOffsetRandomizer : BaseRandomizer<DateTimeOffset>, IRangedRandomizer<DateTimeOffset>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random DateTime value between DateTime.MinValue and today.
    /// </summary>
    /// <returns>A randomly generated DateTime value.</returns>
    public override DateTimeOffset GetRandomValue() => GetRandomValue(DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

    /// <summary>
    ///     Generates a random DateTime value between provided dates.
    /// </summary>
    /// <param name="minValue">Minimum DateTime.</param>
    /// <param name="maxValue">Maximum DateTime.</param>
    /// <returns>A randomly generated DateTime value.</returns>
    public override DateTimeOffset GetRandomValue(DateTimeOffset minValue, DateTimeOffset maxValue)
    {
        var range = (maxValue - minValue).Days;

        return minValue.AddDays(_random.Next(range));
    }
}
