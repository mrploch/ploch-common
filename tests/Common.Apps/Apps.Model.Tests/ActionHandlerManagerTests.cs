using FluentAssertions;
using Ploch.Common.Apps.Model;

namespace Ploch.Common.Apps.Model.Tests;

public class ActionHandlerManagerTests
{
    [Fact]
    public async Task ExecuteAsync_should_return_success_with_handler_execution_id_when_handler_succeeds()
    {
        var handler = new SuccessHandler();
        var manager = new ActionHandlerManager<TestDescriptor, TestActionInfo, SuccessHandler>([handler]);
        var actionInfo = new TestActionInfo(new TestDescriptor(), "test-action");

        var result = await manager.ExecuteAsync(actionInfo, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeTrue();
        result.ExecutionId.ActionInfo.Should().BeSameAs(actionInfo);
        result.ExecutionId.HandlerType.Should().Be<SuccessHandler>();
        result.Errors.Should().BeNull();

        var handlerResult = result.HandlerResults.Should().ContainSingle().Subject;
        handlerResult.IsSuccess.Should().BeTrue();
        handlerResult.ExecutionId.Should().BeSameAs(result.ExecutionId);
    }

    [Fact]
    public async Task ExecuteAsync_should_return_failure_with_error_details_when_all_handlers_fail()
    {
        var handler = new FailureHandler();
        var manager = new ActionHandlerManager<TestDescriptor, TestActionInfo, FailureHandler>([handler]);
        var actionInfo = new TestActionInfo(new TestDescriptor(), "test-action");

        var result = await manager.ExecuteAsync(actionInfo, TestContext.Current.CancellationToken);

        result.IsSuccess.Should().BeFalse();
        result.ExecutionId.ActionInfo.Should().BeSameAs(actionInfo);
        result.ExecutionId.HandlerType.Should().Be<ActionHandlerManager<TestDescriptor, TestActionInfo, FailureHandler>>();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be($"All handlers failed to execute {actionInfo}");

        var handlerResult = result.HandlerResults.Should().ContainSingle().Subject;
        handlerResult.IsSuccess.Should().BeFalse();
        handlerResult.ExecutionId.ActionInfo.Should().BeSameAs(actionInfo);
        handlerResult.ExecutionId.HandlerType.Should().Be<FailureHandler>();
        handlerResult.Errors.Should().ContainSingle().Which.Message.Should().Be("test failure");
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
