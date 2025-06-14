using System.Collections.Generic;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides a generic equality comparer that compares objects by their property values rather than by reference.
///     This allows comparing objects of the same type based on their actual data content.
/// </summary>
/// <typeparam name="TObject">The type of objects to compare.</typeparam>
public class ByValueObjectComparer<TObject> : IEqualityComparer<TObject>
{
    /// <summary>
    ///     Determines whether two objects of type <typeparamref name="TObject" /> are equal by comparing their property values.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    ///     <c>true</c> if the specified objects have equal property values; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(TObject? x, TObject? y) => ByValueObjectComparator.AreEqual(x, y);

    /// <summary>
    ///     Calculates a hash code for the specified object based on its property values.
    ///     This ensures that objects with the same property values will generate the same hash code.
    /// </summary>
    /// <param name="obj">The object for which to calculate a hash code.</param>
    /// <returns>
    ///     A hash code for the specified object, calculated from its property values.
    ///     Returns 0 if the object is null, the type's hash code if the object has no properties,
    ///     or a composite hash code based on all property values.
    /// </returns>
    public int GetHashCode(TObject? obj)
    {
        if (obj is null)
        {
            return 0;
        }

        var type = typeof(TObject);
        var properties = type.GetProperties();

        if (properties.Length == 0)
        {
            return type.GetHashCode();
        }

        var firstPropertyValue = properties[0].GetValue(obj);
        var hashCode = firstPropertyValue?.GetHashCode() ?? 0;

        for (var i = 1; i < properties.Length; i++)
        {
            var propertyInfo = properties[i];
            var value = propertyInfo.GetValue(obj);

            hashCode = (hashCode * 13) + value?.GetHashCode() ?? 0;
        }

        return hashCode;
    }
}
