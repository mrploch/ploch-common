using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Results;

/// <summary>
///     Represents information about an error that occurred during application execution.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="ErrorInfo" /> class.
/// </remarks>
/// <param name="message">The error message describing what went wrong. Cannot be null.</param>
/// <param name="errorCode">An optional error code that can be used to identify the error type.</param>
/// <param name="exception">An optional exception that caused the error.</param>
public class ErrorInfo(string message, string? errorCode, Exception? exception)
{
    /// <summary>
    ///     Gets the error message describing what went wrong.
    /// </summary>
    public string Message { get; } = message.NotNull(nameof(message));

    /// <summary>
    ///     Gets the error code that can be used to identify the error type.
    /// </summary>
    public string? ErrorCode { get; } = errorCode;

    /// <summary>
    ///     Gets or sets the exception that caused the error.
    /// </summary>
    public Exception? Exception { get; set; } = exception;

    /// <summary>
    ///     Creates an error information object from an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the error. Cannot be null.</param>
    /// <param name="errorCode">An optional error code that can be used to identify the error type.</param>
    /// <returns>A new <see cref="ErrorInfo" /> instance containing information from the exception.</returns>
    public static ErrorInfo Create(Exception exception, string? errorCode = null) => new(exception.NotNull(nameof(exception)).Message, errorCode, exception);

    /// <summary>
    ///     Creates an error information object with a custom message and optional exception.
    /// </summary>
    /// <param name="message">The error message describing what went wrong.</param>
    /// <param name="exception">An optional exception that caused the error.</param>
    /// <param name="errorCode">An optional error code that can be used to identify the error type.</param>
    /// <returns>A new <see cref="ErrorInfo" /> instance with the specified message, exception, and error code.</returns>
    public static ErrorInfo Create(string message, Exception? exception, string? errorCode) => new(message, errorCode, exception);

    /// <summary>
    ///     Creates an error information object with just a message and optional error code.
    /// </summary>
    /// <param name="message">The error message describing what went wrong.</param>
    /// <param name="errorCode">An optional error code that can be used to identify the error type.</param>
    /// <returns>A new <see cref="ErrorInfo" /> instance with the specified message and error code.</returns>
    public static ErrorInfo Create(string message, string? errorCode = null) => new(message, errorCode, null);
}
