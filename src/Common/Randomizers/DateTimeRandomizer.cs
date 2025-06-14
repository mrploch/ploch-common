using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random past DateTime values (up until now).
/// </summary>
public class DateTimeRandomizer : BaseRandomizere<DateTime>, IRangedRandomizer<DateTime>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random DateTime value between DateTime.MinValue and today.
    /// </summary>
    /// <returns>A randomly generated DateTime value.</returns>
    public override DateTime GetRandomValue()
    {
        return GetRandomValue(DateTime.MinValue, DateTime.MaxValue);
    }

    /// <summary>
    ///     Generates a random DateTime value between provided dates.
    /// </summary>
    /// <param name="minValue">Minimum DateTime.</param>
    /// <param name="maxValue">Maximum DateTime.</param>
    /// <returns>A randomly generated DateTime value.</returns>
    public override DateTime GetRandomValue(DateTime minValue, DateTime maxValue)
    {
        var range = (maxValue - minValue).Days;

        return minValue.AddDays(_random.Next(range));
    }
}
