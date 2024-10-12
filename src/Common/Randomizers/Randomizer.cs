using System;

namespace Ploch.Common.Randomizers;

/// <summary>
/// Factory for random value generators.
/// </summary>
/// <remarks>
/// Class provides a factory method to get an instance of <see cref="IRandomizer{TValue}"/> for a given type.
/// </remarks>
public static class Randomizer
{
    /// <summary>
    /// Returns an instance of <see cref="IRandomizer{TValue}"/> for the specified type.
    /// </summary>
    /// <typeparam name="TValue">Type to return a randomizer for.</typeparam>
    /// <returns>An instance of a <see cref="IRandomizer{TValue}"/> for the given type.</returns>
    /// <exception cref="NotSupportedException">Thrown when there is no randomizer for the given type.</exception>
    public static IRandomizer<TValue> GetRandomizer<TValue>()
    {
        return typeof(TValue) switch
               {
                   { } t when t == typeof(string) => (IRandomizer<TValue>)new StringRandomizer(),
                   { } t when t == typeof(int) => (IRandomizer<TValue>)new IntRandomizer(),
                   { } t when t == typeof(DateTime) => (IRandomizer<TValue>)new DateTimeRandomizer(),
                   { } t when t == typeof(bool) => (IRandomizer<TValue>)new BooleanRandomizer(),
                   _ => throw new NotSupportedException($"Randomizer for type {typeof(TValue)} is not supported.")
               };
    }
}