using System;

namespace Ploch.Common.Reflection;

/// <summary>
///     Represents an exception that is thrown when there is an error accessing a property.
/// </summary>
public class PropertyAccessException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyAccessException" /> class with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that caused the exception.</param>
    public PropertyAccessException(string propertyName) : base($"Failed to access property {propertyName}") => PropertyName = propertyName;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyAccessException" /> class with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that caused the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public PropertyAccessException(string propertyName, string message) : this(propertyName, message, null)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyAccessException" /> class with the specified property name, error message, and a reference
    ///     to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="propertyName">The name of the property that caused the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PropertyAccessException(string propertyName, string message, Exception? innerException) : base(message, innerException) =>
        PropertyName = propertyName;

    /// <summary>
    ///     Gets the name of the property that caused the exception.
    /// </summary>
    /// <value>The name of the property.</value>
    public string PropertyName { get; }
}
