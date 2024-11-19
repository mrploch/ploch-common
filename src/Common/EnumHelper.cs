using System;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common;

/// <summary>
///     Provides helper methods for working with enums.
/// </summary>
public static class EnumHelper
{
    /// <summary>
    ///     Gets all enum entries.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <returns>All enum entries.</returns>
    public static IEnumerable<TEnum> GetEnumEntries<TEnum>()
        where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
    }
}