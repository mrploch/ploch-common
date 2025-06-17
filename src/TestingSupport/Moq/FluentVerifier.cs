using System;
using System.Threading.Tasks;
using FluentAssertions.Execution;

namespace Ploch.TestingSupport.Moq;

/// <summary>
///     Provides utility methods for verifying fluent assertions within a scoped assertion context.
/// </summary>
public class FluentVerifier
{
    /// <summary>
    ///     Allows using FluentAssertions within Moq verification.
    /// </summary>
    /// <param name="assertion">An action representing the assertion to evaluate.</param>
    /// <returns>True if the assertion passed without any failures; otherwise, false.</returns>
    public static bool VerifyFluentAssertion(Action assertion)
    {
        using var assertionScope = new AssertionScope();
        assertion();

        return assertionScope.Discard().Length == 0;
    }

    /// <summary>
    ///     Allows using FluentAssertions within Moq verification for asynchronous assertions.
    /// </summary>
    /// <param name="assertion">A function representing the asynchronous assertion to evaluate.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is true if the assertion passed without any failures; otherwise, false.</returns>
    public static async Task<bool> VerifyFluentAssertion(Func<Task> assertion)
    {
        using var assertionScope = new AssertionScope();
        await assertion();

        return assertionScope.Discard().Length == 0;
    }
}
