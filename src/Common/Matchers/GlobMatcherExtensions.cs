using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Ploch.Common.Matchers;

/// <summary>
///     Provides extension methods for the <see cref="Matcher" /> class to streamline the construction of the <see cref="Matcher" />.
/// </summary>
public static class GlobMatcherExtensions
{
    /// <summary>
    ///     Adds include patterns to the matcher from an enumerable collection, returning the same matcher provided to the method.
    /// </summary>
    /// <remarks>
    ///     This method is a simple shortcut to streamline the construction of the <see cref="Matcher" />. It allows include and exclude patterns
    ///     to be added in a fluent manner, enabling method chaining, like in the example below.
    /// </remarks>
    /// <example>
    ///     <code lang="csharp">
    ///         var matcher = new Matcher(StringComparison.CurrentCulture).IncludePatterns(["*abc*", "def**"]).ExcludePatterns("**xyz*", "*ghi*");
    ///     </code>
    /// </example>
    /// <param name="matcher">The matcher to add include patterns to.</param>
    /// <param name="patterns">The collection of glob patterns to include.</param>
    /// <returns>The same matcher instance for method chaining.</returns>
    public static Matcher IncludePatterns(this Matcher matcher, IEnumerable<string> patterns) => IncludePatterns(matcher, patterns.ToArray());

    /// <summary>
    ///     Adds include patterns to the matcher.
    /// </summary>
    /// <remarks>
    ///     This method is a simple shortcut to streamline the construction of the <see cref="Matcher" />. It allows include and exclude patterns
    ///     to be added in a fluent manner, enabling method chaining, like in the example below.
    /// </remarks>
    /// <example>
    ///     <code lang="csharp">
    ///         var matcher = new Matcher(StringComparison.CurrentCulture).IncludePatterns("*abc*", "def**").ExcludePatterns("**xyz*", "*ghi*");
    ///     </code>
    /// </example>
    /// <param name="matcher">The matcher to add include patterns to.</param>
    /// <param name="patterns">The glob patterns to include.</param>
    /// <returns>The same matcher instance for method chaining.</returns>
    public static Matcher IncludePatterns(this Matcher matcher, params string[] patterns)
    {
        matcher.AddIncludePatterns(patterns);

        return matcher;
    }

    /// <summary>
    ///     Adds exclude patterns to the matcher from an enumerable collection.
    /// </summary>
    /// <remarks>
    ///     This method is a simple shortcut to streamline the construction of the <see cref="Matcher" />. It allows include and exclude patterns
    ///     to be added in a fluent manner, enabling method chaining, like in the example below.
    /// </remarks>
    /// <example>
    ///     <code lang="csharp">
    ///         var matcher = new Matcher(StringComparison.CurrentCulture).IncludePatterns(["*abc*", "def**"]).ExcludePatterns("**xyz*", "*ghi*");
    ///     </code>
    /// </example>
    /// <param name="matcher">The matcher to add exclude patterns to.</param>
    /// <param name="patterns">The collection of glob patterns to exclude.</param>
    /// <returns>The same matcher instance for method chaining.</returns>
    public static Matcher ExcludePatterns(this Matcher matcher, IEnumerable<string> patterns) => ExcludePatterns(matcher, patterns.ToArray());

    /// <summary>
    ///     Adds exclude patterns to the matcher.
    /// </summary>
    /// <remarks>
    ///     This method is a simple shortcut to streamline the construction of the <see cref="Matcher" />. It allows include and exclude patterns
    ///     to be added in a fluent manner, enabling method chaining, like in the example below.
    /// </remarks>
    /// <example>
    ///     <code lang="csharp">
    ///         var matcher = new Matcher(StringComparison.CurrentCulture).IncludePatterns(["*abc*", "def**"]).ExcludePatterns("**xyz*", "*ghi*");
    ///     </code>
    /// </example>
    /// <param name="matcher">The matcher to add exclude patterns to.</param>
    /// <param name="patterns">The glob patterns to exclude.</param>
    /// <returns>The same matcher instance for method chaining.</returns>
    public static Matcher ExcludePatterns(this Matcher matcher, params string[] patterns)
    {
        matcher.AddExcludePatterns(patterns);

        return matcher;
    }
}
