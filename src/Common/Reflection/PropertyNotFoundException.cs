using System;
using System.Diagnostics.CodeAnalysis;

namespace Ploch.Common.Reflection;

/// <summary>
///     Represents an exception that is thrown when a property is not found during reflection operations.
/// </summary>
[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "This exception is always constructed from a property name; the parameterless and message-only constructors are intentionally omitted because it is derived from PropertyAccessException and is not meaningful without the property context.")]
public class PropertyNotFoundException : PropertyAccessException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyNotFoundException" /> class with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that was not found.</param>
    public PropertyNotFoundException(string propertyName) : base(propertyName, $"Property {propertyName} was not found.")
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyNotFoundException" /> class with the specified property name, error message, and inner
    ///     exception.
    /// </summary>
    /// <param name="propertyName">The name of the property that was not found.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PropertyNotFoundException(string propertyName, string message, Exception innerException) : base(propertyName, message, innerException)
    { }
}
