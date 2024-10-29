using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random past DateTime values (up until now).
/// </summary>
public class DateTimeRandomizer : IRangedRandomizer<DateTime>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random DateTime value between DateTime.MinValue and today.
    /// </summary>
    /// <returns>A randomly generated DateTime value.</
    public DateTime GetRandomValue()
    {
        return GetRandomValue(DateTime.MinValue, DateTime.MaxValue);
    }

    public DateTime GetRandomValue(DateTime minValue, DateTime maxValue)
    {
        var range = (maxValue - minValue).Days;
        return minValue.AddDays(_random.Next(range));
    }
}