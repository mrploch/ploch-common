using System;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.Collections;

/// <summary>
///     Various <see cref="IEnumerable{T}" /> queries written as extension methods.
/// </summary>
public static class EnumerableQueries
{
    /// <summary>
    ///     Filters the input sequence to return only items where the specified property is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the input sequence.</typeparam>
    /// <param name="items">The input sequence to filter.</param>
    /// <param name="propertySelector">A function to extract the string property to check for emptiness.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing only the items where the specified property is empty.</returns>
    public static IEnumerable<T> GetWithEmptyProperty<T>(this IEnumerable<T> items, Func<T, string?> propertySelector) =>
        items.Where(i => string.IsNullOrWhiteSpace(propertySelector(i)));
}
