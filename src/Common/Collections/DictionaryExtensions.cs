using System;
using System.Collections.Generic;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Collections;

/// <summary>
///     Provides extension methods for <see cref="IDictionary{TKey,TValue}" /> to simplify common operations.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Adds multiple key-value pairs to a dictionary with options for handling duplicate keys.
    /// </summary>
    /// <typeparam name="TDictionary">The dictionary type.</typeparam>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to add items to.</param>
    /// <param name="items">The collection of key-value pairs to add to the dictionary.</param>
    /// <param name="duplicateHandling">Specifies how to handle duplicate keys: throw an exception, overwrite existing values, or ignore duplicates.</param>
    /// <returns>The dictionary with added items.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="dictionary" /> or <paramref name="items" /> is null.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when a duplicate key is encountered and <paramref name="duplicateHandling" /> is set to
    ///     <see cref="DuplicateHandling.Throw" />.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="duplicateHandling" /> has an invalid value.</exception>
    public static IDictionary<TKey, TValue> AddMany<TDictionary, TKey, TValue>(this TDictionary dictionary,
                                                                               IEnumerable<KeyValuePair<TKey, TValue>> items,
                                                                               DuplicateHandling duplicateHandling = DuplicateHandling.Throw)
        where TDictionary : class, IDictionary<TKey, TValue>
    {
        _ = dictionary.NotNull(nameof(dictionary));
        _ = items.NotNull(nameof(items));
        _ = duplicateHandling.NotOutOfRange(nameof(duplicateHandling));

        foreach (var item in items)
        {
            if (dictionary.ContainsKey(item.Key))
            {
                switch (duplicateHandling)
                {
                    case DuplicateHandling.Overwrite:
                        dictionary[item.Key] = item.Value;

                        break;
                    case DuplicateHandling.Throw:
                        throw new ArgumentException($"An item with the key '{item.Key}' already exists in the dictionary.", nameof(items));
                    case DuplicateHandling.Ignore:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(duplicateHandling), duplicateHandling, null);
                }
            }
            else
            {
                dictionary.Add(item.Key, item.Value);
            }
        }

        return dictionary;
    }
}
