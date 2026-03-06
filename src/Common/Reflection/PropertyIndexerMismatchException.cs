using System;

namespace Ploch.Common.Reflection;

/// <summary>
///     Represents an exception that is thrown when there is a mismatch in property indexer access.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="PropertyIndexerMismatchException" /> class with the specified error message and a reference to the inner
///     exception that is the cause of this exception.
/// </remarks>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class PropertyIndexerMismatchException(string message, Exception? innerException)
    : PropertyAccessException(PropertyHelpers.IndexerPropertyName, message, innerException)
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PropertyIndexerMismatchException" /> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public PropertyIndexerMismatchException(string message) : this(message, null)
    { }
}
