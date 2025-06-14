using System.Collections.Generic;

namespace Ploch.Common.Matchers;

/// <summary>
///     Provides a contract for evaluating a collection of matchers to determine if a provided string meets the criteria defined by the matchers.
/// </summary>
/// <typeparam name="TMatcher">The type of matcher used to evaluate strings.</typeparam>
public interface IMatchersListEvaluator<TMatcher> : IStringMatcher
{
    /// <summary>
    ///     Represents a collection of matchers and provides functionality to evaluate a given value against the matchers' criteria.
    /// </summary>
    IEnumerable<TMatcher> Matchers { get; }

    /// <summary>
    ///     Determines whether a specified string value matches any of the criteria defined by the matchers in the list.
    /// </summary>
    /// <param name="value">The string value to evaluate against the matchers.</param>
    /// <returns>
    ///     <c>true</c> if the provided string matches any of the criteria defined by the matchers; otherwise, <c>false</c>.
    /// </returns>
    bool IsMatch(string value);
}
