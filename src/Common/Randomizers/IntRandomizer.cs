using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random integer values.
/// </summary>
public class IntRandomizer : IRangedRandomizer<int>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random integer value.
    /// </summary>
    /// <returns>A randomly generated integer value.</returns>
    public int GetRandomValue()
    {
        return _random.Next();
    }

    /// <summary>
    ///     Generates a random integer value within the specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
    /// <returns>A randomly generated integer value between minValue (inclusive) and maxValue (exclusive).</returns>
    public int GetRandomValue(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}