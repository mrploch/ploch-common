using Ploch.Common.TypeConversion;

namespace Ploch.Common.Tests.TypeConversion;

public class TypeConversionExceptionTests
{
    [Fact]
    public void Constructor_with_value_and_targetType_should_build_message_and_set_properties()
    {
        var exception = new TypeConversionException("42", typeof(int));

        Assert.Equal("42", exception.ConvertedValue);
        Assert.Equal(typeof(int), exception.TargetType);
        Assert.Contains("42", exception.Message);
        Assert.Contains(typeof(int).ToString(), exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_with_value_targetType_and_innerException_should_set_all_properties()
    {
        var inner = new FormatException("bad format");

        var exception = new TypeConversionException("abc", typeof(int), inner);

        Assert.Equal("abc", exception.ConvertedValue);
        Assert.Equal(typeof(int), exception.TargetType);
        Assert.Same(inner, exception.InnerException);
        Assert.Contains("abc", exception.Message);
    }

    [Fact]
    public void Constructor_with_message_value_and_targetType_should_use_provided_message()
    {
        const string message = "Custom error message";

        var exception = new TypeConversionException(message, "value", typeof(double));

        Assert.Equal(message, exception.Message);
        Assert.Equal("value", exception.ConvertedValue);
        Assert.Equal(typeof(double), exception.TargetType);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_with_all_parameters_should_set_all_properties()
    {
        const string message = "Custom error";
        var inner = new InvalidCastException("cast failed");

        var exception = new TypeConversionException(message, 123, typeof(string), inner);

        Assert.Equal(message, exception.Message);
        Assert.Equal(123, exception.ConvertedValue);
        Assert.Equal(typeof(string), exception.TargetType);
        Assert.Same(inner, exception.InnerException);
    }
}
