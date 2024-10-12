using System;

namespace Ploch.Common.Randomizers;

/// <summary>
/// Provides functionality to generate random boolean values.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IRandomizer{Boolean}"/> interface to generate random boolean values.
/// </remarks>
public class BooleanRandomizer : IRandomizer<bool>
{
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random boolean value.
    /// </summary>
    /// <returns>A randomly generated boolean value.</returns>
    /// <remarks>
    /// This method uses the <see cref="System.Random"/> class to generate a boolean value.
    /// </remarks>
    public bool GetValue()
    {
        return Random.NextDouble() >= 0.5;
    }
}