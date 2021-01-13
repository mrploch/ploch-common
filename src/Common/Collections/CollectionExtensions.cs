using System;
using System.Collections.Generic;

namespace Ploch.Common.Collections
{
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Adds a pair of key / value to a collection based on <see cref="KeyValuePair{TKey,TValue}" /> items.
        /// </summary>
        /// <typeparam name="TCollection">The collection type.</typeparam>
        /// <typeparam name="TKey">First parameter type (key).</typeparam>
        /// <typeparam name="TValue">Second parameter type (value)</typeparam>
        /// <param name="collection">The collection instance to add key and value to.</param>
        /// <param name="key">First parameter (key) value.</param>
        /// <param name="value">Second parameter (value) value.</param>
        /// <returns>Same instance of collection that values were added to, providing fluent interface.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is <see langword="null" />.</exception>
        public static TCollection Add<TCollection, TKey, TValue>(this TCollection collection, TKey key, TValue value)
            where TCollection: ICollection<KeyValuePair<TKey, TValue>>
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            collection.Add(new KeyValuePair<TKey, TValue>(key, value));
            return collection;
        }

        /// <summary>Adds all items to the collection.</summary>
        /// <typeparam name="TItem">Item type</typeparam>
        /// <param name="collection">The collection instance</param>
        /// <param name="items">Items to add to the collection</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection" /> or <paramref name="items" /> is
        ///     <see langword="null" />
        /// </exception>
        public static void AddMany<TItem>(this ICollection<TItem> collection, params TItem[] items)
        {
            AddManyInternal(collection, items);
        }

        /// <summary>Adds all items to the collection.</summary>
        /// <typeparam name="TItem">Item type</typeparam>
        /// <param name="collection">The collection instance</param>
        /// <param name="items">Items to add to the collection</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection" /> or <paramref name="items" /> is
        ///     <see langword="null" />
        /// </exception>
        public static void AddMany<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
        {
            AddManyInternal(collection, items);
        }

        /// <summary>
        /// Adds items to a collection
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="collection" /> or <paramref name="items" /> is
        ///     <see langword="null" />
        /// </exception>
        private static void AddManyInternal<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}