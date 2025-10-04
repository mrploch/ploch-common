namespace Ploch.Common.Tests;

public class StopwatchUtilTests
{
    [Fact]
    public void Time_should_return_action_execution_time_for()
    {
#pragma warning disable S2925 // Do not use Thread.Sleep in tests
        var action = () => Thread.Sleep(TimeSpan.FromMilliseconds(100));
#pragma warning restore S2925

        var actionTime = StopwatchUtil.Time(action);

        actionTime.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public async Task Time_should_return_execution_time_for_task_func()
    {
        static Task TaskAsync() => Task.Delay(TimeSpan.FromMilliseconds(100));
        var taskTime = await StopwatchUtil.TimeAsync(TaskAsync);

        taskTime.Should().BeGreaterThan(TimeSpan.FromMilliseconds(90));
    }

    [Fact]
    public async Task Time_should_return_execution_time_for_started_taskAsync()
    {
        var asyncMethodTime = await StopwatchUtil.TimeAsync(AsyncMethod(TimeSpan.FromMilliseconds(100)));

        asyncMethodTime.Should().BeGreaterThan(TimeSpan.FromMilliseconds(90));
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
#pragma warning disable CA1822
#pragma warning disable CC0091
#pragma warning disable CC0061
#pragma warning disable S2325
    private Task AsyncMethod(TimeSpan delay) => Task.Delay(delay);
#pragma warning restore S2325
#pragma warning restore CC0061
#pragma warning restore CC0091
#pragma warning restore CA1822
}
