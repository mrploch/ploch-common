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
        // TimeAsync(Task) starts measuring when it is called, so a task handed to it that is
        // already counting down a fixed delay can finish before - or shortly after - measurement
        // begins. Any lower bound derived from that delay is therefore a race against how loaded
        // the machine is, which is what made this test fail intermittently (see issue #299).
        //
        // Gate completion on a signal this test controls instead. TimeAsync runs synchronously up
        // to its first await, so its stopwatch is already running by the time the call returns a
        // pending task; everything awaited afterwards falls inside the measurement window. The
        // bound below can then only be exceeded, never undershot.
        var completionSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var timing = StopwatchUtil.TimeAsync(completionSignal.Task);

        await Task.Delay(TimeSpan.FromMilliseconds(200));
        completionSignal.SetResult(true);

        var startedTaskTime = await timing;

        // Generous margin below the 200 ms actually waited, to absorb timer resolution and any
        // scheduling delay between the call and the stopwatch starting.
        startedTaskTime.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(100));
    }
}
