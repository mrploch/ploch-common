using System;

namespace Ploch.Common.Randomizers;

/// <summary>
/// Provides functionality to generate random integer values.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IRandomizer{Int32}"/> interface to generate random integers.
/// </remarks>
public class IntRandomizer : IRandomizer<int>
{
    private static readonly Random Random = new();

    /// <summary>
    /// Generates a random integer value.
    /// </summary>
    /// <returns>A randomly generated integer value.</returns>
    /// <remarks>
    /// This method uses the <see cref="System.Random"/> class to generate a random integer.
    /// </remarks>
    public int GetValue()
    {
        return Random.Next();
    }
}