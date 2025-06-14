using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Base abstract class for randomizers that generate random values of type <typeparamref name="TValue" />.
/// </summary>
/// <typeparam name="TValue">The type of values this randomizer generates.</typeparam>
public abstract class BaseRandomizere<TValue> : IRangedRandomizer<TValue>
{
    /// <summary>
    ///     Generates a random value within the specified range.
    /// </summary>
    /// <param name="minValue">The minimum value (inclusive) of the range.</param>
    /// <param name="maxValue">The maximum value (inclusive) of the range.</param>
    /// <returns>A random value of type <typeparamref name="TValue" /> within the specified range.</returns>
    public abstract TValue GetRandomValue(TValue minValue, TValue maxValue);

    /// <summary>
    ///     Generates a random value within the specified range using object parameters.
    /// </summary>
    /// <param name="minValue">The minimum value (inclusive) of the range as an object.</param>
    /// <param name="maxValue">The maximum value (inclusive) of the range as an object.</param>
    /// <returns>A random value as an object within the specified range.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the randomizer returns a null value.</exception>
    public object GetRandomValue(object minValue, object maxValue) =>
        GetRandomValue((TValue)minValue, (TValue)maxValue) ?? throw new InvalidOperationException("Randomizer returned null value.");

    /// <summary>
    ///     Generates a random value using default range settings.
    /// </summary>
    /// <returns>A random value of type <typeparamref name="TValue" />.</returns>
    public abstract TValue GetRandomValue();

    /// <summary>
    ///     Explicit interface implementation of IRandomizer.GetRandomValue that generates a random value.
    /// </summary>
    /// <returns>A random value as an object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the randomizer returns a null value.</exception>
    object IRandomizer.GetRandomValue() => GetRandomValue() ?? throw new InvalidOperationException("Randomizer returned null value.");
}
