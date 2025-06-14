using System;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides methods for comparing objects by their property values rather than by reference.
/// </summary>
public static class ByValueObjectComparator
{
    /// <summary>
    ///     Determines whether two objects are equal by comparing their property values recursively.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <param name="type">The type to use for property comparison. If null, the type of the first object will be used.</param>
    /// <returns>
    ///     <c>true</c> if both objects are null or if all their property values are equal;
    ///     <c>false</c> if only one object is null or if any of their property values differ.
    /// </returns>
    public static bool AreEqual(object? x, object? y, Type? type = null)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        type ??= x.GetType();

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == PropertyHelpers.IndexerPropertyName)
            {
                continue;
            }

            var xValue = property.GetValue(x);
            var yValue = property.GetValue(y);

            if (property.PropertyType.IsSimpleType())
            {
                if (!Equals(xValue, yValue))
                {
                    return false;
                }
            }
            else
            {
                if (!AreEqual(xValue, yValue, property.PropertyType))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
