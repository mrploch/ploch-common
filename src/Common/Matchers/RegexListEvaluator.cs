using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ploch.Common.Matchers;

/// <summary>
///     Evaluates string values against a list of regular expressions.
/// </summary>
/// <remarks>
///     This class provides functionality to match strings against multiple regular expressions.
///     It converts a collection of regex patterns into compiled Regex objects and provides
///     methods to check if a string matches any of the patterns.
/// </remarks>
/// <param name="regexList">A collection of regular expression patterns to match against.</param>
/// <param name="nullValueMatchResult">A match value to return if the checked value is <c>null</c></param>
/// <param name="compiled">Indicates whether to compile the regular expressions for improved performance. Default is true.</param>
/// <param name="ignoreCase">Indicates whether case should be ignored when matching. Default is true.</param>
public class RegexListEvaluator(IEnumerable<string> regexList, bool nullValueMatchResult = false, bool compiled = true, bool ignoreCase = true) : IStringMatcher
{
    private readonly IEnumerable<Regex> _matchers = regexList.Select(regexString =>
                                                                     {
                                                                         var options = RegexOptions.None;
                                                                         if (compiled)
                                                                         {
                                                                             options |= RegexOptions.Compiled;
                                                                         }

                                                                         if (ignoreCase)
                                                                         {
                                                                             options |= RegexOptions.IgnoreCase;
                                                                         }

                                                                         return new Regex(regexString, options);
                                                                     });

    /// <summary>
    ///     Determines whether the specified string matches any of the regular expressions in the collection.
    /// </summary>
    /// <remarks>
    ///     If the input string is null, the method returns the value of <see cref="nullValueMatchResult" />.
    ///     Otherwise, it checks if the string matches any of the compiled regular expressions.
    /// </remarks>
    /// <param name="value">The string to match against the regular expressions.</param>
    /// <returns>
    ///     <c>true</c> if the string matches any of the regular expressions; if value is null then the value of <see cref="nullValueMatchResult" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool IsMatch(string? value) => value == null ? nullValueMatchResult : _matchers.Any(regex => regex.IsMatch(value));
}
