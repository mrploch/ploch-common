using Ploch.Common.Apps.Model;

namespace Ploch.Common.Apps.Model.Tests;

public class ActionInfoTests
{
    [Fact]
    public void Constructor_should_set_Descriptor_and_Name()
    {
        var descriptor = new TestDescriptor();
        const string name = "my-action";

        var actionInfo = new ActionInfo<TestDescriptor>(descriptor, name);

        Assert.Same(descriptor, actionInfo.Descriptor);
        Assert.Equal(name, actionInfo.Name);
    }

    [Fact]
    public void Name_should_return_the_value_provided_in_constructor()
    {
        var actionInfo = new ActionInfo<TestDescriptor>(new TestDescriptor(), "another-action");

        Assert.Equal("another-action", actionInfo.Name);
    }

    private sealed class TestDescriptor : IActionTargetDescriptor
    {
        public string Name => "TestApp";
    }
}
