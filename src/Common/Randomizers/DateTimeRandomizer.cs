using System;

namespace Ploch.Common.Randomizers;

/// <summary>
/// Provides functionality to generate random <see cref="DateTime"/> values.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IRandomizer{DateTime}"/> interface to generate random dates.
/// </remarks>
public class DateTimeRandomizer : IRandomizer<DateTime>
{
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random <see cref="DateTime"/> value.
    /// </summary>
    /// <returns>A randomly generated <see cref="DateTime"/> value.</returns>
    /// <remarks>
    /// This method generates a random date between <see cref="DateTime.MinValue"/> and today's date.
    /// </remarks>
    public DateTime GetValue()
    {
        var range = (DateTime.Today - DateTime.MinValue).Days;
        return DateTime.MinValue.AddDays(Random.Next(range));
    }
}