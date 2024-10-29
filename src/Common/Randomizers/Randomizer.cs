using System;

namespace Ploch.Common.Randomizers;

/// <summary>
///     Provides functionality to get a randomizer instance for various types such as string, int, DateTime, and bool.
/// </summary>
public static class Randomizer
{
    /// <summary>
    ///     Retrieves an instance of <see cref="IRandomizer{TValue}" /> appropriate for the specified type parameter.
    ///     Supported types are string, int, DateTime, and bool.
    /// </summary>
    /// <typeparam name="TValue">The type for which to retrieve a randomizer instance.</typeparam>
    /// <returns>An instance of <see cref="IRandomizer{TValue}" /> for the specified type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the type parameter is not supported.</exception>
    public static IRangedRandomizer<TValue> GetRandomizer<TValue>()
    {
        return typeof(TValue) switch
               {
                   { } t when t == typeof(string) => (IRangedRandomizer<TValue>)new StringRandomizer(),
                   { } t when t == typeof(int) => (IRangedRandomizer<TValue>)new IntRandomizer(),
                   { } t when t == typeof(DateTime) => (IRangedRandomizer<TValue>)new DateTimeRandomizer(),
                   { } t when t == typeof(bool) => (IRangedRandomizer<TValue>)new BooleanRandomizer(),
                   _ => throw new NotSupportedException($"Randomizer for type {typeof(TValue)} is not supported.")
               };
    }
}