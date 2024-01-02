using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dawn;

namespace Ploch.Common.Collections;

/// <summary>
///     Extension methods for <see cref="ICollection{T}" />.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    ///     Adds a pair of key / value to a collection or a dictionary if the <paramref name="value" /> is not null.
    /// </summary>
    /// <remarks>
    ///     This method also provides a fluent interface by returning the same instance of <paramref name="collection" /> that
    ///     values were added to.
    /// </remarks>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     the value of
    ///     <param name="collection"></param>
    ///     .
    /// </returns>
    public static ICollection<KeyValuePair<TKey, TValue?>> AddIfNotNull<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue?>> collection, TKey key, TValue? value)
        where TValue : class?
    {
        Guard.Argument(collection, nameof(collection)).NotNull();

        if (value != null)
        {
            collection.Add(key, value);
        }

        return collection;
    }

    /// <summary>
    ///     Adds a pair of key / value to a collection or a dictionary items.
    /// </summary>
    /// <remarks>
    ///     This method also provides a fluent interface by returning the same instance of <paramref name="collection" /> that
    ///     values were added to.
    /// </remarks>
    /// <typeparam name="TKey">The type of <paramref name="key" /> parameter.</typeparam>
    /// <typeparam name="TValue">The type of <paramref name="value" /> parameter.</typeparam>
    /// <param name="collection">The collection or dictionary instance to add key and value to.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>Same instance of collection that values were added to, providing fluent interface.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="collection" /> is <see langword="null" />.</exception>
    public static ICollection<KeyValuePair<TKey, TValue?>> Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue?>> collection, TKey key, TValue? value)
    {
        Guard.Argument(collection, nameof(collection)).NotNull();

        collection.Add(new KeyValuePair<TKey, TValue?>(key, value));

        return collection;
    }

    /// <summary>Adds all items to the collection.</summary>
    /// <typeparam name="TItem">Item type.</typeparam>
    /// <param name="collection">The collection instance.</param>
    /// <param name="items">Items to add to the collection.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="collection" /> or <paramref name="items" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <returns>The source collection after addition, providing a fluent interface.</returns>
    public static ICollection<TItem> AddMany<TItem>(this ICollection<TItem> collection, params TItem[] items)
    {
        AddManyInternal(collection, items);

        return collection;
    }

    /// <summary>Adds all items to the collection.</summary>
    /// <typeparam name="TItem">Item type.</typeparam>
    /// <param name="collection">The collection instance.</param>
    /// <param name="items">Items to add to the collection.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="collection" /> or <paramref name="items" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <returns>The source collection after addition, providing a fluent interface.</returns>
    public static ICollection<TItem> AddMany<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
    {
        AddManyInternal(collection, items);

        return collection;
    }

    /// <summary>
    ///     Adds items to a collection.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <param name="collection">The collection to add items to.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="collection" /> or <paramref name="items" /> is
    ///     <see langword="null" />.
    /// </exception>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "False positive - Guard.Argument would not enumerate collection.")]
    private static void AddManyInternal<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
    {
        Guard.Argument(items, nameof(items)).NotNull();
        Guard.Argument(collection, nameof(collection)).NotNull();

        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
}