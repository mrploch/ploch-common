using System;
using System.Collections.Generic;
using System.Globalization;
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
    public static IEnumerable<TEnum> GetEnumEntries<TEnum>() where TEnum : Enum => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    /// <summary>
    ///     Returns all enumeration flags of the specified value.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The enum value to extract flags from.</param>
    /// <returns>A collection of all non-zero flags present in the given value.</returns>
    /// <remarks>
    ///     Uses <see cref="Enum.HasFlag(Enum)" /> to determine which flags are set and excludes the zero value.
    ///     Useful for working with enums marked with the <see cref="FlagsAttribute" />.
    /// </remarks>
    public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum value) where TEnum : Enum => GetEnumEntries<TEnum>()
                                                                                             .Where(flag => value.HasFlag(flag) &&
                                                                                                            Convert.ToInt32(flag,
                                                                                                                            CultureInfo.InvariantCulture) != 0)
                                                                                             .ToList();
}
