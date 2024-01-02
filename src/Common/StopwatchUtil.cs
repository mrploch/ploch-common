using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Dawn;

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
        Guard.Argument(action, nameof(action)).NotNull();

        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        return sw.Elapsed;
    }

    public static TimeSpan Time(Task task)
    {
        Guard.Argument(task, nameof(task)).NotNull();

        var sw = Stopwatch.StartNew();
        task.Wait();
        sw.Stop();

        return sw.Elapsed;
    }

    public static TimeSpan Time(Func<Task> asyncAction)
    {
        Guard.Argument(asyncAction, nameof(asyncAction)).NotNull();

        var sw = Stopwatch.StartNew();
        var task = asyncAction();
        task.Wait();
        sw.Stop();

        return sw.Elapsed;
    }
}