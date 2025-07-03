using System;

namespace Ploch.Common.Reflection;

/// <summary>
///     Represents an exception that is thrown when an attempt is made to write to a read-only property.
/// </summary>
/// <remarks>
///     This exception is derived from <see cref="PropertyAccessException" /> and provides additional
///     context specific to read-only property access violations.
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="PropertyReadOnlyException" /> class with the specified property name, error message, and optional
///     inner exception.
/// </remarks>
/// <param name="propertyName">The name of the read-only property that caused the exception.</param>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class PropertyReadOnlyException(string propertyName, string message, Exception? innerException = null)
    : PropertyAccessException(propertyName, message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyReadOnlyException" /> class with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the read-only property that caused the exception.</param>
    public PropertyReadOnlyException(string propertyName) : this(propertyName, $"Property {propertyName} is read-only.")
    { }
}
