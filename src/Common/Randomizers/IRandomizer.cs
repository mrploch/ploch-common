namespace Ploch.Common.Randomizers;

/// <summary>
///     Represents an interface for generating random values of a specified type.
/// </summary>
/// <typeparam name="TValue">The type of values to be generated.</typeparam>
public interface IRandomizer<out TValue>
{
    /// <summary>
    ///     Generates a random value of the specified type.
    /// </summary>
    /// <returns>A randomly generated value of the specified type.</returns>
    TValue GetRandomValue();
}