using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to generate random boolean values.
/// </summary>
public class BooleanRandomizer : BaseRandomizer<bool>, IRangedRandomizer<bool>
{
    private readonly Random _random = new();

    /// <summary>
    ///     Generates a random boolean value.
    /// </summary>
    /// <returns>A randomly generated boolean value.</returns>
    public override bool GetRandomValue() => _random.Next(0, 2) == 1;

    /// <summary>
    ///     Generates a random boolean value between the specified range.
    /// </summary>
    /// <param name="minValue">The minimum boolean value, inclusive.</param>
    /// <param name="maxValue">The maximum boolean value, inclusive.</param>
    /// <returns>A randomly generated boolean value between the specified range.</returns>
    public override bool GetRandomValue(bool minValue, bool maxValue) => minValue == maxValue ? maxValue : GetRandomValue();
}
