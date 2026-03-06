using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Specifies the name of the corresponding property in a source object to map to during type conversion.
///     This attribute is used to indicate which property of a source object should be used when mapping
///     to a property of a different name in the destination object.
/// </summary>
/// <example>
///     For example, if you have a source object with a property named "OldPropertyName" and you want to map
///     its value to a property named "NewPropertyName" in the destination object, you would decorate the
///     "NewPropertyName" property with this attribute, specifying "OldPropertyName" as the
///     <see cref="PropertyName" />.
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ObjectPropertyAttribute(string propertyName) : Attribute
{
    /// <summary>
    ///     Gets the name of the property in the source object.
    /// </summary>
    public string PropertyName { get; } = propertyName.NotNull(nameof(propertyName));
}
