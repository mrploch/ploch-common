using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random boolean values.
/// </summary>
public class BooleanRandomizer : IRangedRandomizer<bool>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random boolean value.
    /// </summary>
    /// <returns>A randomly generated boolean value.</returns>
    public bool GetRandomValue()
    {
        return _random.Next(0, 2) == 1;
    }

    public bool GetRandomValue(bool minValue, bool maxValue)
    {
        return minValue == maxValue ? maxValue : GetRandomValue();
    }
}