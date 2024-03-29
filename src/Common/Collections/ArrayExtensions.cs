﻿using System;
using Dawn;

namespace Ploch.Common.Collections;

/// <summary>
///     Extension methods for <see cref="Array" /> class.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    ///     Checks if the array contains an item matching the specified predicate.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="predicate">The predicate for an item.</param>
    /// <typeparam name="TItem">The array item type.</typeparam>
    /// <returns><c>true</c> if array contains matching item, <c>false</c> otherwise.</returns>
    public static bool Exists<TItem>(this TItem[] array, Predicate<TItem> predicate)
    {
        Guard.Argument(array, nameof(array)).NotNull();
        Guard.Argument(predicate, nameof(predicate)).NotNull();

        return Array.Exists(array, predicate);
    }
}