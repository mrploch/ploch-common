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

    public int GetRandomValue(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}