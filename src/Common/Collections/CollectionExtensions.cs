using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Ploch.Common.ArgumentChecking;

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
    ///     the value of <paramref name="collection" />.
    /// </returns>
    public static ICollection<KeyValuePair<TKey, TValue?>> AddIfNotNull<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue?>> collection,
                                                                                      TKey key,
                                                                                      TValue? value) where TValue : class?
    {
        collection.NotNull(nameof(collection));

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
        collection.NotNull(nameof(collection));

        collection.Add(new(key, value));

        return collection;
    }

    /// <summary>
    ///     Adds multiple items to a collection. Provides control over handling duplicates using the <paramref name="duplicateHandling" /> parameter.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection being extended, which implements <see cref="ICollection{T}" />.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <param name="collection">The target collection to which items are added.</param>
    /// <param name="duplicateHandling">
    ///     Specifies how duplicate items should be handled:
    ///     <see cref="DuplicateHandling.Ignore" />, <see cref="DuplicateHandling.Overwrite" />, or <see cref="DuplicateHandling.Throw" />.
    /// </param>
    /// <param name="items">The items to add to the collection.</param>
    /// <returns>The same instance of <paramref name="collection" /> after items have been added.</returns>
    public static ICollection<TItem> AddMany<TCollection, TItem>(this TCollection collection,
                                                                 DuplicateHandling duplicateHandling = DuplicateHandling.Throw,
                                                                 params TItem[] items) where TCollection : ICollection<TItem>
    {
        collection.AddManyInternal(items.NotNull(nameof(items)), duplicateHandling);

        return collection;
    }

    /// <summary>Adds all items to the collection.</summary>
    /// <param name="collection">The collection instance.</param>
    /// <param name="items">Items to add to the collection.</param>
    /// <param name="duplicateHandling">Duplicate handling behavior.</param>
    /// <typeparam name="TCollection">The collection type.</typeparam>
    /// <typeparam name="TItem">Item type.</typeparam>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="collection" /> or <paramref name="items" /> is
    ///     <see langword="null" />.
    /// </exception>
    /// <returns>The source collection after addition, providing a fluent interface.</returns>
    public static TCollection AddMany<TCollection, TItem>(this TCollection collection,
                                                          IEnumerable<TItem> items,
                                                          DuplicateHandling duplicateHandling = DuplicateHandling.Throw) where TCollection : ICollection<TItem>
    {
        collection.AddManyInternal(items.NotNull(nameof(items)), duplicateHandling);

        return collection;
    }

    /// <summary>
    ///     Adds items to a collection.
    /// </summary>
    /// <typeparam name="TItem">The type of items.</typeparam>
    /// <param name="collection">The collection to add items to.</param>
    /// <param name="items">The items.</param>
    /// <param name="duplicateHandling">Duplicate handling behavior.</param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="collection" /> or <paramref name="items" /> is
    ///     <see langword="null" />.
    /// </exception>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "False positive - Guard.Argument would not enumerate collection.")]
    private static void AddManyInternal<TItem>(this ICollection<TItem> collection,
                                               IEnumerable<TItem> items,
                                               DuplicateHandling duplicateHandling = DuplicateHandling.Throw)
    {
        items.NotNull(nameof(items));
        collection.NotNull(nameof(collection));

        foreach (var item in items)
        {
            if (collection.Contains(item))
            {
                switch (duplicateHandling)
                {
                    case DuplicateHandling.Throw:
                        throw new ArgumentException($"Item {item} already exists in the collection.", nameof(items));
                    case DuplicateHandling.Ignore:
                        continue;
                    case DuplicateHandling.Overwrite:
                        // Pointless in most cases for a normal collection (not a key/value pair collection), it'll just remove and add the same item.
                        // Might be useful only if a type contains a custom equality comparer or things like cache that refreshes item access.
                        collection.Remove(item);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(duplicateHandling), duplicateHandling, null);
                }
            }

            collection.Add(item);
        }
    }
}
