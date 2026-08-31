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

        // Thread.Sleep is only guaranteed to sleep *about* the requested time: the OS timer granularity is ~15.6 ms on
        // Windows, so a 100 ms sleep can be measured at ~85 ms. Asserting the full 100 ms is a race, which is part of
        // what made this class fail intermittently (see issue #299). The bound below still proves the elapsed time
        // tracks the action rather than being zero or a constant.
        actionTime.Should().BeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public async Task Time_should_return_execution_time_for_task_func()
    {
        static Task TaskAsync() => Task.Delay(TimeSpan.FromMilliseconds(100));
        var taskTime = await StopwatchUtil.TimeAsync(TaskAsync);

        // Task.Delay fires on a timer whose resolution is ~15.6 ms on Windows, so a 100 ms delay can complete at
        // ~85 ms; the original 90 ms bound left only 10 ms of slack (see issue #299).
        taskTime.Should().BeGreaterThan(TimeSpan.FromMilliseconds(50));
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
