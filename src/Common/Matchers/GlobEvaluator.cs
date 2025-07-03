using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.Collections;

namespace Ploch.Common.Matchers;
// TODO: I think this should be moved to a separate project due to the dependency on Microsoft.Extensions.FileSystemGlobbing.

/// <summary>
///     Evaluates strings against a list of glob patterns to determine if they match.
///     Uses include and exclude patterns to define matching criteria.
/// </summary>
/// <param name="includes">Collection of glob patterns that should be included in the match.</param>
/// <param name="excludes">Collection of glob patterns that should be excluded from the match.</param>
/// <param name="nullMatchResult">Determines the result when the input value is null. Default is false.</param>
/// <param name="comparisonType">The string comparison type to use when matching. Default is StringComparison.OrdinalIgnoreCase.</param>
public class GlobEvaluator(IEnumerable<string> includes,
                           IEnumerable<string> excludes,
                           bool nullMatchResult = false,
                           bool emptyMatchResult = false,
                           StringComparison comparisonType = StringComparison.OrdinalIgnoreCase) : IStringMatcher
{
    private readonly Matcher _matcher = new Matcher(comparisonType).IncludePatterns(includes).ExcludePatterns(excludes);

    /// <summary>
    ///     Determines whether the specified string matches any of the include patterns and none of the exclude patterns.
    /// </summary>
    /// <param name="value">The string to evaluate against the glob patterns.</param>
    /// <returns>
    ///     True if the string matches any include pattern and no exclude patterns, or if the string is null and nullMatchResult is true.
    ///     False otherwise.
    /// </returns>
    public bool IsMatch(string? value)
    {
        if (value == null)
        {
            return nullMatchResult;
        }

        if (value.IsEmpty())
        {
            return emptyMatchResult;
        }

        var result = _matcher.Match(value);

        return result.HasMatches;
    }
}
