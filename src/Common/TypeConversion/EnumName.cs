using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Initializes a new instance of the <see cref="EnumName" /> record.
/// </summary>
/// <param name="Name">The name of the enum value.</param>
/// <param name="CaseSensitive">A value indicating whether the comparison should be case-sensitive. Defaults to <c>false</c>.</param>
public record EnumName(string? Name, bool CaseSensitive = false)
{
    /// <summary>
    ///     Gets the name of the enum value.
    /// </summary>
    public string? Name { get; } = Name;

    /// <summary>
    ///     Gets a value indicating whether the comparison should be case-sensitive.
    /// </summary>
    public bool CaseSensitive { get; } = CaseSensitive;

    /// <summary>
    ///     Converts a string to an <see cref="EnumName" /> instance.
    /// </summary>
    /// <param name="name">The string to convert.</param>
    /// <returns>An <see cref="EnumName" /> instance initialized with the specified string.</returns>
    public static implicit operator EnumName(string? name) => new(name);

    /// <summary>
    ///     Converts an <see cref="EnumName" /> instance to its string representation.
    /// </summary>
    /// <param name="enumName">The <see cref="EnumName" /> instance to convert. This must not be null.</param>
    /// <returns>The string representation of the <see cref="EnumName" /> instance.</returns>
    public static explicit operator string?(EnumName enumName) => enumName.NotNull(nameof(enumName)).Name;

    /// <summary>
    ///     Determines whether an <see cref="EnumName" /> is equal to a string.
    /// </summary>
    /// <param name="left">The <see cref="EnumName" /> to compare.</param>
    /// <param name="right">The string to compare.</param>
    /// <returns><c>true</c> if the <see cref="EnumName" /> is equal to the string; otherwise, <c>false</c>.</returns>
    public static bool operator ==(EnumName left, string? right)
    {
        if (right is null && left.NotNull(nameof(left)).Name is null)
        {
            return true;
        }

        if (right.IsNullOrEmpty() && left.NotNull(nameof(left)).Name.IsNullOrEmpty())
        {
            return true;
        }

        if (left.NotNull(nameof(left)).Name.IsNullOrEmpty())
        {
            return false;
        }

        return right.NotNull(nameof(right))
                    .Equals(left.NotNull(nameof(left)).Name, left.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Determines whether an <see cref="EnumName" /> is not equal to a string.
    /// </summary>
    /// <param name="left">The <see cref="EnumName" /> to compare.</param>
    /// <param name="right">The string to compare.</param>
    /// <returns><c>true</c> if the <see cref="EnumName" /> is not equal to the string; otherwise, <c>false</c>.</returns>
    public static bool operator !=(EnumName left, string? right)
    {
        if (right is null && left.NotNull(nameof(left)).Name is null)
        {
            return false;
        }

        if (right.IsNullOrEmpty() && left.NotNull(nameof(left)).Name.IsNullOrEmpty())
        {
            return false;
        }

        if (left.NotNull(nameof(left)).Name.IsNullOrEmpty())
        {
            return false;
        }

        return !right.NotNull(nameof(right))
                     .Equals(left.NotNull(nameof(left)).Name, left.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }

    //
    // public override bool Equals(object obj)
    // {
    //     if (obj is EnumName other)
    //     {
    //         return Name == other.Name;
    //     }
    //
    //     return false;
    // }
    // //
    // public override int GetHashCode()
    // {
    //     unchecked
    //     {
    //         int hashcode = 1430287;
    //         hashcode = hashcode * 7302013 ^ Name.GetHashCode();
    //         hashcode = hashcode * 7302013 ^ CaseSensitive.GetHashCode();
    //         return hashcode;
    //     }
    // }

    // public virtual bool Equals(EnumName? other)
    // {
    //     var equals = base.Equals(other);
    //
    //     return equals;
    // }
}
