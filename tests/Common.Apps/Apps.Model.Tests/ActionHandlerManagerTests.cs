using Ploch.Common.Apps.Model;

namespace Ploch.Common.Apps.Model.Tests;

public class ActionHandlerManagerTests
{
    [Fact]
    public async Task ExecuteAsync_should_return_success_when_handler_succeeds()
    {
        var handler = new SuccessHandler();
        var manager = new ActionHandlerManager<TestDescriptor, TestActionInfo, SuccessHandler>([handler]);
        var actionInfo = new TestActionInfo(new TestDescriptor(), "test-action");

        var result = await manager.ExecuteAsync(actionInfo);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_should_return_failure_when_all_handlers_fail()
    {
        var handler = new FailureHandler();
        var manager = new ActionHandlerManager<TestDescriptor, TestActionInfo, FailureHandler>([handler]);
        var actionInfo = new TestActionInfo(new TestDescriptor(), "test-action");

        var result = await manager.ExecuteAsync(actionInfo);

        Assert.False(result.IsSuccess);
    }

    private sealed class TestDescriptor : IActionTargetDescriptor
    {
        public string Name => "TestApp";
    }

    private sealed class TestActionInfo : ActionInfo<TestDescriptor>, IActionInfo<IActionTargetDescriptor>
    {
        private readonly TestDescriptor _descriptor;

        public TestActionInfo(TestDescriptor descriptor, string name) : base(descriptor, name)
        {
            _descriptor = descriptor;
        }

        IActionTargetDescriptor IActionInfo<IActionTargetDescriptor>.Descriptor => _descriptor;
    }

    private sealed class SuccessHandler : ActionHandler<TestDescriptor, TestActionInfo>
    {
        public override int Priority => 0;

        public override Task<ActionHandlerResult<TestDescriptor>> ExecuteAsync(TestActionInfo actionInfo, CancellationToken cancellationToken = default)
            => Task.FromResult(ActionHandlerResult.Success(new ActionExecutionId<TestDescriptor>(actionInfo, GetType())));
    }

    private sealed class FailureHandler : ActionHandler<TestDescriptor, TestActionInfo>
    {
        public override int Priority => 0;

        public override Task<ActionHandlerResult<TestDescriptor>> ExecuteAsync(TestActionInfo actionInfo, CancellationToken cancellationToken = default)
            => Task.FromResult(ActionHandlerResult.Failure(new ActionExecutionId<TestDescriptor>(actionInfo, GetType()), "test failure"));
    }
}
