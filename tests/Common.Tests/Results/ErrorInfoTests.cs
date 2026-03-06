using Ploch.Common.Results;

namespace Ploch.Common.Tests.Results;

public class ErrorInfoTests
{
    [Fact]
    public void Constructor_should_create_error_info_with_all_parameters()
    {
        // Arrange
        var message = "Something went wrong";
        var errorCode = "ERR123";
        var exception = new InvalidOperationException("Invalid operation");

        // Act
        var errorInfo = new ErrorInfo(message, errorCode, exception);

        // Assert
        errorInfo.Message.Should().Be(message);
        errorInfo.ErrorCode.Should().Be(errorCode);
        errorInfo.Exception.Should().Be(exception);
    }

    [Fact]
    public void Create_should_create_error_info_with_message_only()
    {
        // Arrange
        var message = "Only message";

        // Act
        var errorInfo = ErrorInfo.Create(message);

        // Assert
        errorInfo.Message.Should().Be(message);
        errorInfo.ErrorCode.Should().BeNull();
        errorInfo.Exception.Should().BeNull();
    }

    [Fact]
    public void Create_should_create_error_info_from_exception()
    {
// Parameter name in ArgumentNullException should match actual parameter - does not apply to this test
#pragma warning disable S3928
        var exception = new ArgumentNullException("param", "Parameter is required");
#pragma warning restore S3928
        var errorCode = "EXC001";

        // Act
        var errorInfo = ErrorInfo.Create(exception, errorCode);

        // Assert
        errorInfo.Message.Should().Be(exception.Message);
        errorInfo.ErrorCode.Should().Be(errorCode);
        errorInfo.Exception.Should().Be(exception);
    }

    [Fact]
    public void Constructor_should_throw_when_message_is_null()
    {
        // Arrange
        string? message = null;

        // Act
        var act = () => new ErrorInfo(message!, "ERR", new());

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(message));
    }

    [Fact]
    public void Create_should_throw_when_exception_is_null()
    {
        // Arrange
        Exception? exception = null;

        // Act
        var act = () => ErrorInfo.Create(exception!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("exception");
    }

    [Fact]
    public void Exception_property_should_allow_setting_to_null()
    {
        // Arrange
        var errorInfo = new ErrorInfo("msg", "code", new("ex")) { Exception = null };

        // Assert
        errorInfo.Exception.Should().BeNull();
    }

    [Fact]
    public void Create_should_set_message_and_null_error_code_when_only_exception_provided()
    {
        // Arrange
        var exception = new InvalidOperationException("Operation failed");

        // Act
        var errorInfo = ErrorInfo.Create(exception);

        // Assert
        errorInfo.Message.Should().Be(exception.Message);
        errorInfo.ErrorCode.Should().BeNull();
        errorInfo.Exception.Should().Be(exception);
    }

    [Fact]
    public void Create_should_preserve_original_exception_in_error_info()
    {
        // Arrange
        var exception = new InvalidOperationException("Original exception");

        // Act
        var errorInfo = ErrorInfo.Create(exception);

        // Assert
        errorInfo.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void Create_should_handle_exception_with_empty_message()
    {
        // Arrange
#pragma warning disable CA2201 // Exception is not specific enough
        var exception = new Exception(string.Empty);
#pragma warning restore CA2201

        // Act
        var errorInfo = ErrorInfo.Create(exception);

        // Assert
        errorInfo.Message.Should().Be(string.Empty);
        errorInfo.Exception.Should().Be(exception);
    }

    [Fact]
    public void Create_should_throw_if_exception_message_is_null()
    {
        // Arrange
        var exception = new ExceptionWithNullMessage();

        // Act
        Action act = () => ErrorInfo.Create(exception);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("message");
    }

    [Fact]
    public void Create_should_create_error_info_with_message_exception_and_error_code()
    {
        // Arrange
        var message = "Custom error message";
        var exception = new InvalidOperationException("Operation failed");
        var errorCode = "ERR456";

        // Act
        var errorInfo = ErrorInfo.Create(message, exception, errorCode);

        // Assert
        errorInfo.Message.Should().Be(message);
        errorInfo.Exception.Should().Be(exception);
        errorInfo.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void Create_should_handle_null_exception_parameter()
    {
        // Arrange
        var message = "Custom error message";
        Exception? exception = null;
        var errorCode = "ERR789";

        // Act
        var errorInfo = ErrorInfo.Create(message, exception, errorCode);

        // Assert
        errorInfo.Message.Should().Be(message);
        errorInfo.Exception.Should().BeNull();
        errorInfo.ErrorCode.Should().Be(errorCode);
    }

    [Fact]
    public void Create_should_handle_empty_error_code_parameter()
    {
        // Arrange
        var message = "Custom error message";
        var exception = new InvalidOperationException("Operation failed");
        var errorCode = string.Empty;

        // Act
        var errorInfo = ErrorInfo.Create(message, exception, errorCode);

        // Assert
        errorInfo.Message.Should().Be(message);
        errorInfo.Exception.Should().Be(exception);
        errorInfo.ErrorCode.Should().Be(string.Empty);
    }

    // Helper classes for edge cases
    private class ExceptionWithNullMessage : Exception
    {
        public override string Message => null!;
    }

    private class MyCustomException : Exception
    {
        public MyCustomException(string message) : base(message)
        { }
    }
}
