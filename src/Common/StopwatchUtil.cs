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
    ///     Measures the time it takes to execute the specified action.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <returns>The time it took to execute the action.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the action is null.</exception>
    /// <example>
    ///     <code>
    /// // Measure the time it takes to execute a method
    /// TimeSpan executionTime = Time(() =>
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
    public static TimeSpan Time(Task task)
    {
#pragma warning disable VSTHRD110
        task.NotNull(nameof(task));
#pragma warning restore VSTHRD110

        var sw = Stopwatch.StartNew();
        task.Wait();
        sw.Stop();

        return sw.Elapsed;
    }

    /// <summary>
    ///     Measures the time it takes to execute the specified asynchronous action.
    /// </summary>
    /// <param name="asyncAction">The asynchronous action to measure the time for.</param>
    /// <returns>The time elapsed for the action to complete.</returns>
    public static TimeSpan Time(Func<Task> asyncAction)
    {
        asyncAction.NotNull(nameof(asyncAction));

        var sw = Stopwatch.StartNew();
        var task = asyncAction();
        task.Wait();
        sw.Stop();

        return sw.Elapsed;
    }
}
