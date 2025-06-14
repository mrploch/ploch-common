namespace Ploch.Common.Randomizers;

public interface IRangeRandomizer : IRandomizer
{
    /// <summary>
    ///     Generates a random value of the specified type within the provided range.
    /// </summary>
    /// <param name="minValue">The minimum value of the generated range.</param>
    /// <param name="maxValue">The maximum value of the generated range.</param>
    /// <returns>A randomly generated value of the specified type within the range.</returns>
    object GetRandomValue(object minValue, object maxValue);
}

/// <summary>
///     Represents an interface for generating random values of a specified type within a defined range.
/// </summary>
/// <typeparam name="TValue">The type of values to be generated.</typeparam>
public interface IRangedRandomizer<TValue> : IRandomizer<TValue>, IRangeRandomizer
{
    /// <summary>
    ///     Generates a random value of the specified type within the provided range.
    /// </summary>
    /// <param name="minValue">The minimum value of the generated range.</param>
    /// <param name="maxValue">The maximum value of the generated range.</param>
    /// <returns>A randomly generated value of the specified type within the range.</returns>
    TValue GetRandomValue(TValue minValue, TValue maxValue);
}
