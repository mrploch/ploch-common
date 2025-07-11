using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides extension methods for collections of <see cref="EnumName" /> values.
/// </summary>
public static class EnumNameEnumerableExtensions
{
    /// <summary>
    ///     Determines whether any <see cref="EnumName" /> in the collection matches the specified name.
    /// </summary>
    /// <param name="enumNames">The collection of enum names to search.</param>
    /// <param name="name">The name to search for.</param>
    /// <returns><c>true</c> if the collection contains a matching enum name; otherwise, <c>false</c>.</returns>
    public static bool Contains(this IEnumerable<EnumName> enumNames, string name) => enumNames.Any(e => e == name);
}
