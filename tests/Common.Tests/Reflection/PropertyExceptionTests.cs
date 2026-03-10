using Ploch.Common.Reflection;

namespace Ploch.Common.Tests.Reflection;

public class PropertyExceptionTests
{
    [Fact]
    public void PropertyNotFoundException_with_propertyName_should_set_message_and_PropertyName()
    {
        var exception = new PropertyNotFoundException("MyProp");

        Assert.Equal("MyProp", exception.PropertyName);
        Assert.Contains("MyProp", exception.Message);
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void PropertyNotFoundException_with_all_parameters_should_set_all_properties()
    {
        var inner = new InvalidOperationException("inner error");

        var exception = new PropertyNotFoundException("MyProp", "Custom message", inner);

        Assert.Equal("MyProp", exception.PropertyName);
        Assert.Equal("Custom message", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void PropertyReadOnlyException_with_propertyName_should_set_default_message()
    {
        var exception = new PropertyReadOnlyException("ReadOnlyProp");

        Assert.Equal("ReadOnlyProp", exception.PropertyName);
        Assert.Contains("ReadOnlyProp", exception.Message);
        Assert.Contains("read-only", exception.Message);
    }

    [Fact]
    public void PropertyReadOnlyException_with_propertyName_and_message_should_use_provided_message()
    {
        var exception = new PropertyReadOnlyException("Prop", "Cannot write to this");

        Assert.Equal("Prop", exception.PropertyName);
        Assert.Equal("Cannot write to this", exception.Message);
    }

    [Fact]
    public void PropertyReadOnlyException_with_all_parameters_should_set_innerException()
    {
        var inner = new AccessViolationException("access error");

        var exception = new PropertyReadOnlyException("Prop", "Error", inner);

        Assert.Equal("Prop", exception.PropertyName);
        Assert.Equal("Error", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void PropertyAccessException_with_propertyName_only_should_set_default_message()
    {
        var exception = new PropertyAccessException("SomeProp");

        Assert.Equal("SomeProp", exception.PropertyName);
        Assert.Contains("SomeProp", exception.Message);
    }

    [Fact]
    public void PropertyAccessException_with_propertyName_and_message_should_use_provided_message()
    {
        var exception = new PropertyAccessException("SomeProp", "Custom access error");

        Assert.Equal("SomeProp", exception.PropertyName);
        Assert.Equal("Custom access error", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void PropertyAccessException_with_all_parameters_should_set_all_properties()
    {
        var inner = new Exception("root cause");

        var exception = new PropertyAccessException("Prop", "Error occurred", inner);

        Assert.Equal("Prop", exception.PropertyName);
        Assert.Equal("Error occurred", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }
}
