using System;
using System.Collections.Generic;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides equality comparison for instances of <see cref="EnumName" />.
/// </summary>
/// <remarks>
///     This comparer is designed to compare <see cref="EnumName" /> instances using both the name
///     and the case-sensitivity configuration. It evaluates equality based on the string value
///     of the name and whether the comparison is case-sensitive or not.
/// </remarks>
public class EnumNameValueComparer : IEqualityComparer<EnumName>
{
    /// <summary>
    ///     Determines whether two <see cref="EnumName" /> instances are considered equal based on their properties.
    /// </summary>
    /// <param name="x">The first <see cref="EnumName" /> instance to compare.</param>
    /// <param name="y">The second <see cref="EnumName" /> instance to compare.</param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="EnumName" /> instances are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(EnumName? x, EnumName? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return (bool)y?.Name.IsNullOrEmpty()!;
        }

        if (y is null)
        {
            return x.Name.IsNullOrEmpty();
        }

        if (x.Name.IsNullOrEmpty())
        {
            return y.Name.IsNullOrEmpty();
        }

        return x.Name!.Equals(y.Name, x.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Generates a hash code for the specified <see cref="EnumName" /> object based on its properties.
    /// </summary>
    /// <param name="obj">The <see cref="EnumName" /> instance for which to generate a hash code.</param>
    /// <returns>An integer representing the hash code of the specified <see cref="EnumName" /> instance.</returns>
    public int GetHashCode(EnumName obj)
    {
        if (obj.NotNull(nameof(obj)).Name is null)
        {
            return 0;
        }

        return obj.CaseSensitive ? StringComparer.Ordinal.GetHashCode(obj.Name!) : StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name!);
    }
}
