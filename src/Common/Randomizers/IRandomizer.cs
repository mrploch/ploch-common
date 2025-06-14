namespace Ploch.Common.Randomizers;

/// <summary>
///     Defines the functionality for generating random values and handling range-specific constraints for a specified type.
/// </summary>
public interface IRandomizer
{
    /// <summary>
    ///     Generates a random value of the specified type within the provided range.
    /// </summary>
    /// <param name="minValue">The minimum value of the generated range.</param>
    /// <param name="maxValue">The maximum value of the generated range.</param>
    /// <returns>A randomly generated value of the specified type within the range.</returns>
    object GetRandomValue(object minValue, object maxValue);

    /// <summary>
    ///     Generates a random value of the randomizer type.
    /// </summary>
    /// <returns>A randomly generated value of the randomizer type.</returns>
    object GetRandomValue();
}

/// <summary>
///     Represents an interface for generating random values of a specified type.
/// </summary>
/// <typeparam name="TValue">The type of values to be generated.</typeparam>
public interface IRandomizer<out TValue> : IRandomizer
{
    /// <summary>
    ///     Generates a random value of the specified type.
    /// </summary>
    /// <returns>A randomly generated value of the specified type.</returns>
    TValue GetRandomValue();
}
