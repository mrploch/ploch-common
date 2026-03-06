using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Primitives;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.FluentAssertions;

/// <summary>
///     Provides extension methods for <see cref="StringAssertions" />.
/// </summary>
public static class StringAssertionExtensions
{
    /// <summary>
    ///     Asserts that the subject string contains all of the specified values ignoring case.
    /// </summary>
    /// <param name="assertions">The assertions.</param>
    /// <param name="values">Values to check.</param>
    /// <returns>The assertion constraint.</returns>
    public static AndConstraint<StringAssertions> ContainAllEquivalentOf(this StringAssertions assertions, params string?[] values) =>
        ContainAllEquivalentOf(assertions, values, string.Empty);

    /// <summary>
    ///     Asserts that the subject string contains all of the specified values ignoring case.
    /// </summary>
    /// <param name="assertions">The assertions.</param>
    /// <param name="values">Values to check.</param>
    /// <param name="because">The because string.</param>
    /// <param name="becauseArgs">The because string arguments.</param>
    /// <returns>The assertion constraint.</returns>
    public static AndConstraint<StringAssertions> ContainAllEquivalentOf(this StringAssertions assertions,
                                                                         IEnumerable<string?> values,
                                                                         string because = "",
                                                                         params object[] becauseArgs)
    {
        values.NotNull(nameof(values));
        var array = values.Where(v => !Contains(assertions.Subject, v, StringComparison.OrdinalIgnoreCase)).ToArray();

        assertions.CurrentAssertionChain
                  .ForCondition(values != null && values.Any())
                  .FailWith("You have to provide at least one value to check for.")
                  .Then
                  .ForCondition(values.All(v => Contains(assertions.Subject, v, StringComparison.OrdinalIgnoreCase)))
                  .BecauseOf(because, becauseArgs)
                  .FailWith("Expected {context:string} {0} to contain the strings ignoring case: {1} but {2} was not found{reason}.",
                            assertions.Subject,
                            values,
                            array);

        return new(assertions);
    }

    /// <summary>
    ///     Determines whether the specified string contains the expected substring, using the specified string comparison rules.
    /// </summary>
    /// <param name="actual">The string being evaluated.</param>
    /// <param name="expected">The substring to locate within <paramref name="actual" />.</param>
    /// <param name="comparison">The type of comparison to perform between strings.</param>
    /// <returns><c>true</c> if <paramref name="actual" /> contains <paramref name="expected" /> according to the specified comparison; otherwise, <c>false</c>.</returns>
    private static bool Contains(string? actual,
                                 string? expected,
                                 StringComparison comparison) => (actual ?? string.Empty).IndexOf(expected ?? string.Empty, comparison) >= 0;

    /*
     *   public static bool Contains(this StringAssertions assertions,
                                string? actual,
                                string? expected,
                                StringComparison comparison,
                                string because = "",
                                params object[] becauseArgs)
    {
        assertions.CurrentAssertionChain.ForCondition((actual ?? string.Empty).IndexOf(expected ?? string.Empty, comparison) >= 0)
                  .BecauseOf(because, becauseArgs)
                  .FailWith("Expected {context:string} {0} to contain the strings ignoring case: {1}{reason}.", assertions.Subject, array)

        return (actual ?? string.Empty).IndexOf(expected ?? string.Empty, comparison) >= 0;
    }
     */
}
