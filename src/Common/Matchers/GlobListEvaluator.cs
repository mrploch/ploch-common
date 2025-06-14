using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Ploch.Common.Matchers;

public class GlobListEvaluator : IStringMatcher
{
    private readonly Matcher _matcher;
    private readonly Func<bool> _nullMatcher;

    public GlobListEvaluator(IEnumerable<string> includes,
                             IEnumerable<string> excludes,
                             Func<bool>? nullMatcher = null,
                             StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        _matcher = new Matcher(comparisonType);
        _matcher.AddIncludePatterns(includes);
        _matcher.AddExcludePatterns(excludes);
        _nullMatcher = nullMatcher ?? (() => false);
    }

    public bool IsMatch(string? value)
    {
        if (value == null)
        {
            return _nullMatcher();
        }

        var result = _matcher.Match(value);

        return result.HasMatches;
    }
}
