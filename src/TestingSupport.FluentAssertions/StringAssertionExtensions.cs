using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Ploch.TestingSupport.FluentAssertions
{
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
        public static AndConstraint<StringAssertions> ContainAllEquivalentOf(this StringAssertions assertions, params string?[] values)
        {
            return ContainAllEquivalentOf(assertions, values, string.Empty);
        }

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
            Guard.Argument(values, nameof(values)).NotNull();
            var array = values.Where(v => !Contains(assertions.Subject, v, StringComparison.OrdinalIgnoreCase)).ToArray();
            Execute.Assertion.ForCondition(values.All(v => Contains(assertions.Subject, v, StringComparison.OrdinalIgnoreCase)))
                   .BecauseOf(because, becauseArgs)
                   .FailWith("Expected {context:string} {0} to contain the strings ignoring case: {1}{reason}.", assertions.Subject, array);

            return new AndConstraint<StringAssertions>(assertions);
        }

        private static bool Contains(string? actual, string? expected, StringComparison comparison)
        {
            return (actual ?? string.Empty).IndexOf(expected ?? string.Empty, comparison) >= 0;
        }
    }
}