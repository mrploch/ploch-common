using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Represents a utility class for measuring the execution time of an action using a <see cref="Stopwatch" />.
/// </summary>
public static class StopwatchUtil
{
    /// <summary>
    ///     Measures the time it takes to execute the specified asynchronous action.
    /// </summary>
    /// <param name="asyncAction">The asynchronous action to measure the time for.</param>
    /// <returns>The time elapsed for the action to complete.</returns>
    /// <example>
    ///     <code lang="csharp">
    ///     // Example: Measure the execution time of an asynchronous method
    ///     // This is a synchronous call that blocks until the async operation completes
    ///     TimeSpan elapsed = StopwatchUtil.Time(async () =>
    ///     {
    ///         await Task.Delay(500);
    ///         // do some work here...
    ///     });
    ///     Console.WriteLine($"Operation took {elapsed.TotalMilliseconds} ms");
    /// 
    ///     // Example: Measuring an existing async method
    ///     async Task SomeAsyncOperation()
    ///     {
    ///         await Task.Delay(250);
    ///         // do some work here...
    ///     }
    /// 
    ///     TimeSpan methodElapsed = StopwatchUtil.Time(SomeAsyncOperation);
    ///     Console.WriteLine($"Method took {methodElapsed.TotalMilliseconds} ms");
    ///     </code>
    /// </example>
    /// p
    public static async Task<TimeSpan> TimeAsync(Func<Task> asyncAction)
    {
        asyncAction.NotNull(nameof(asyncAction));

        var sw = Stopwatch.StartNew();
        var task = asyncAction();
        await task;
        sw.Stop();

        return sw.Elapsed;
    }

    /// <summary>
    ///     Measures the time it takes to execute the specified action.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <returns>The time it took to execute the action.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the action is null.</exception>
    /// <example>
    ///     <code>
    /// // Measure the time it takes to execute a method
    /// TimeSpan executionTime = StopwatchUtil.Time(() =>
    /// {
    /// // Code to be executed
    /// });
    /// </code>
    /// </example>
    public static TimeSpan Time(Action action)
    {
        action.NotNull(nameof(action));

        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        return sw.Elapsed;
    }

    /// <summary>
    ///     Measures the time it takes for a task to complete.
    /// </summary>
    /// <param name="task">The task to measure the time for.</param>
    /// <returns>The time elapsed for the task to complete.</returns>
    /// <example>
    ///     <code lang="csharp">
    ///     // Example 1: Measure an existing Task
    ///     // Assume this code is inside an async method
    ///     Task work = Task.Delay(500);
    ///     TimeSpan elapsed = await StopwatchUtil.TimeAsync(work);
    ///     Console.WriteLine($"Work took {elapsed.TotalMilliseconds} ms");
    /// 
    ///     // Example 2: Measure an operation created inline
    ///     TimeSpan inlineElapsed = await StopwatchUtil.TimeAsync(Task.Run(async () =>
    ///     {
    ///         await Task.Delay(250);
    ///         // do some work here...
    ///     }));
    ///     Console.WriteLine($"Inline work took {inlineElapsed.TotalMilliseconds} ms");
    ///     </code>
    /// </example>
    public static async Task<TimeSpan> TimeAsync(Task task)
    {
#pragma warning disable VSTHRD110
        task.NotNull(nameof(task));
#pragma warning restore VSTHRD110

        var configuredTaskAwaitable = task.ConfigureAwait(false);

        var sw = Stopwatch.StartNew();
        await configuredTaskAwaitable;
        sw.Stop();

        return sw.Elapsed;
    }
}
