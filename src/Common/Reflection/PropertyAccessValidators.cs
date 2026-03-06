using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides validation methods to validate the access of properties using <see cref="PropertyInfo" /> objects.
/// </summary>
public static class PropertyAccessValidators
{
    /// <summary>
    ///     Validates if a given <see cref="PropertyInfo" /> represents a property that can be read and optionally validates index parameters if applicable.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo" /> instance representing the property to validate. May be null.</param>
    /// <param name="propertyName">The name of the property being validated. Used for exception messages if <paramref name="propertyInfo" /> is null.</param>
    /// <param name="index">An optional array of values to match the expected index parameters for indexed properties.</param>
    /// <exception cref="PropertyNotFoundException">Thrown when <paramref name="propertyInfo" /> is null.</exception>
    /// <exception cref="PropertyWriteOnlyException">Thrown when the property is write-only and cannot be read.</exception>
    /// <exception cref="PropertyIndexerMismatchException">
    ///     Thrown when the property is an indexed property but no index values are provided, or when the provided index values do not match the expected types.
    /// </exception>
    /// <remarks>
    ///     This method checks the validity of a property for the purpose of retrieving its value.
    ///     It ensures that the property exists, is readable, and, for indexed properties, that the provided index arguments match the expectations.
    /// </remarks>
    /// <example>
    ///     The following example demonstrates how to use <see cref="ValidatePropertyInfoForGetValue" /> to validate a property:
    ///     <code>
    /// // Example setup
    /// var propertyInfo = typeof(SomeClass).GetProperty("SomeProperty");
    /// object[]? indexValues = null; // Set this if dealing with indexed properties
    /// // Validation
    /// try
    /// {
    /// PropertyAccessValidators.ValidatePropertyInfoForGetValue(propertyInfo, "SomeProperty", indexValues);
    /// Console.WriteLine("Property is valid for reading.");
    /// }
    /// catch (Exception ex)
    /// {
    /// Console.WriteLine($"Validation failed: {ex.Message}");
    /// }
    /// </code>
    /// </example>
    public static void ValidatePropertyInfoForGetValue(PropertyInfo? propertyInfo, string propertyName, object?[]? index = null)
    {
        if (propertyInfo == null)
        {
            throw new PropertyNotFoundException(propertyName);
        }

        if (!propertyInfo.CanRead)
        {
            throw new PropertyWriteOnlyException(propertyInfo.Name);
        }

        if (propertyInfo.GetIndexParameters().Length > 0 && index == null)
        {
            throw new PropertyIndexerMismatchException("Index parameters are required for indexed properties.");
        }

        if (index == null)
        {
            return;
        }

        for (var i = 0; i < index.Length; i++)
        {
            if (!propertyInfo.GetIndexParameters()[i].ParameterType.IsInstanceOfType(index[i]))
            {
                throw new PropertyIndexerMismatchException($"Argument {i} is not of the expected type {propertyInfo.GetIndexParameters()[i].ParameterType}");
            }
        }
    }
}
