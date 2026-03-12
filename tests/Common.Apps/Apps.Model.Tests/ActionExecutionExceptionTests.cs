using Ploch.Common.Apps.Model;

namespace Ploch.Common.Apps.Model.Tests;

public class ActionExecutionExceptionTests
{
    [Fact]
    public void Constructor_with_actionInfo_only_should_set_ActionInfo_and_null_message()
    {
        var actionInfo = new TestActionInfo("test-action");

        var exception = new ActionExecutionException(actionInfo);

        Assert.Same(actionInfo, exception.ActionInfo);
        Assert.NotNull(exception.Message); // Exception.Message returns default message when null is passed
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_with_actionInfo_and_message_should_set_both_properties()
    {
        var actionInfo = new TestActionInfo("test-action");
        const string message = "Something went wrong";

        var exception = new ActionExecutionException(actionInfo, message);

        Assert.Same(actionInfo, exception.ActionInfo);
        Assert.Equal(message, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_with_all_parameters_should_set_all_properties()
    {
        var actionInfo = new TestActionInfo("test-action");
        const string message = "Something went wrong";
        var inner = new InvalidOperationException("inner");

        var exception = new ActionExecutionException(actionInfo, message, inner);

        Assert.Same(actionInfo, exception.ActionInfo);
        Assert.Equal(message, exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    private sealed class TestActionInfo(string name) : IActionInfo
    {
        public string Name => name;
    }
}
