using System;
using System.Collections.Generic;
using Dawn;

namespace Ploch.Common.Collections
{
    public static class CollectionExtensions
    {
        /// <summary>Adds a value if value not null.</summary>
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
        public static IDictionary<TKey, TValue> AddIfNotNull<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value) where TValue : class
        {
            Guard.Argument(collection, nameof(collection)).NotNull();

            if (value != null)
            {
                collection.Add(key, value);
            }

            return collection;
        }

        /// <summary>Adds a value if value not null.</summary>
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
        public static ICollection<KeyValuePair<TKey, TValue>> AddIfNotNull<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key, TValue value)
            where TValue : class
        {
            Guard.Argument(collection, nameof(collection)).NotNull();

            if (value != null)
            {
                collection.Add(key, value);
            }

            return collection;
        }

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
        public static ICollection<KeyValuePair<TKey, TValue>> Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key, TValue value)
        {
            Guard.Argument(collection, nameof(collection)).NotNull();

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
            Guard.Argument(collection, nameof(collection)).NotNull();

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
            Guard.Argument(collection, nameof(collection)).NotNull();

            AddManyInternal(collection, items);
        }

        /// <summary>
        ///     Adds items to a collection
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
            Guard.Argument(items, nameof(items)).NotNull();
            Guard.Argument(collection, nameof(collection)).NotNull();

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}