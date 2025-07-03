using System;

namespace Ploch.Common.Reflection;

/// <summary>
///     Represents an exception that is thrown when attempting to read a write-only property.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="PropertyWriteOnlyException" /> class with the specified property name, error message, and optional
///     inner exception.
/// </remarks>
/// <param name="propertyName">The name of the write-only property that caused the exception.</param>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class PropertyWriteOnlyException(string propertyName, string message, Exception? innerException = null)
    : PropertyAccessException(propertyName, message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyWriteOnlyException" /> class with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the write-only property that caused the exception.</param>
    public PropertyWriteOnlyException(string propertyName) : this(propertyName, $"Property {propertyName} is write-only.")
    { }
}
