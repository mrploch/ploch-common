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
        var randomizer = GetRandomizerInternal(typeof(TValue));

        return (IRangedRandomizer<TValue>)randomizer;
    }

    /// <summary>
    ///     Retrieves an instance of <see cref="IRandomizer" /> appropriate for the specified type.
    ///     Supported types are string, int, DateTime, DateTimeOffset, and bool.
    /// </summary>
    /// <param name="type">The type for which to retrieve a randomizer instance.</param>
    /// <returns>An instance of <see cref="IRandomizer" /> for the specified type.</returns>
    /// <exception cref="NotSupportedException">Thrown when the provided type is not supported.</exception>
    public static IRandomizer GetRandomizer(Type type) => GetRandomizerInternal(type);

    private static IRandomizer GetRandomizerInternal(Type type) => type switch
                                                                   { not null when type == typeof(string) => new StringRandomizer(),
                                                                     not null when type == typeof(int) => new IntRandomizer(),
                                                                     not null when type == typeof(DateTime) => new DateTimeRandomizer(),
                                                                     not null when type == typeof(DateTimeOffset) => new DateTimeOffsetRandomizer(),
                                                                     not null when type == typeof(bool) => new BooleanRandomizer(),
                                                                     _ =>
                                                                         throw new NotSupportedException($"Randomizer for type {type} is not supported.") };
}
