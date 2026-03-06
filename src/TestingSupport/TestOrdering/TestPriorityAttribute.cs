using System;

namespace Ploch.TestingSupport.TestOrdering;

/// <summary>
///     Specifies the execution priority of a test method.
/// </summary>
/// <remarks>
///     This attribute can be applied to test methods to control their execution order.
///     Tests with lower priority values are executed before tests with higher priority values.
/// </remarks>
/// <param name="priority">The priority value for the test method. Lower values indicate higher priority.</param>
[AttributeUsage(AttributeTargets.Method)]
public class TestPriorityAttribute(int priority) : Attribute
{
    /// <summary>
    ///     Gets the priority value of the test method.
    /// </summary>
    /// <value>An integer representing the test's execution priority. Lower values indicate higher priority.</value>
    public int Priority { get; } = priority;
}
