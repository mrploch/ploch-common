using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;

namespace TestingSupport.FluentAssertions.IOAbstractions;

/// <summary>
///     Provides a way to do assertions on collections of <see cref="IFileSystemInfo" /> objects.
/// </summary>
/// <typeparam name="TItems">The type of the file system info items in the collection.</typeparam>
/// <remarks>
///     Initializes a new instance of the <see cref="FileSystemInfoAssertions{TItems}" /> class.
/// </remarks>
/// <param name="actualValue">The collection of file system info objects to verify.</param>
public class FileSystemInfoAssertions<TItems>(IEnumerable<TItems> actualValue) : GenericCollectionAssertions<TItems>(actualValue)
    where TItems : IFileSystemInfo
{
    /// <summary>
    ///     Gets the identifier of the subject for use in error messages.
    /// </summary>
    protected override string Identifier => nameof(IFileSystemInfo);

    /// <summary>
    ///     Asserts that the file system info names in the collection are equivalent to the specified names, ignoring case.
    /// </summary>
    /// <param name="fileSystemInfoNames">The expected file system info names.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> which can be used for chaining assertions.</returns>
    public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(params string[] fileSystemInfoNames) =>
        HaveNamesEquivalentToIgnoringCase(fileSystemInfoNames.AsEnumerable());

    /// <summary>
    ///     Asserts that the file system info names in the collection are equivalent to the specified names, ignoring case.
    /// </summary>
    /// <param name="fileSystemInfoNames">The expected file system info names.</param>
    /// <param name="because">
    ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> which can be used for chaining assertions.</returns>
    public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(IEnumerable<string> fileSystemInfoNames,
                                                                                             string because = "",
                                                                                             params object[] becauseArgs) =>
        HaveNamesEquivalentTo(fileSystemInfoNames, StringComparer.OrdinalIgnoreCase, because, becauseArgs);

    /// <summary>
    ///     Asserts that the file system info names in the collection are equivalent to the specified names using the provided
    ///     equality comparer.
    /// </summary>
    /// <param name="fileSystemInfoNames">The expected file system info names.</param>
    /// <param name="equalityComparer">The equality comparer to use for comparing the file system info names.</param>
    /// <param name="because">
    ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
    ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
    /// </param>
    /// <param name="becauseArgs">Zero or more objects to format using the placeholders in <paramref name="because" />.</param>
    /// <returns>An <see cref="AndConstraint{T}" /> which can be used for chaining assertions.</returns>
    public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentTo(IEnumerable<string> fileSystemInfoNames,
                                                                                 StringComparer equalityComparer,
                                                                                 string because = "",
                                                                                 params object[] becauseArgs)
    {
        Subject.Select(fsi => TrimEndPathSeparator(fsi.Name))
               .OrderBy(name => name)
               .Should()
               .BeEquivalentTo(fileSystemInfoNames.Select(TrimEndPathSeparator).OrderBy(static name => name),
                               config => config.Using(equalityComparer),
                               because,
                               becauseArgs);

        return new AndConstraint<FileSystemInfoAssertions<TItems>>(this);
    }

    /// <summary>
    ///     Removes trailing path separators ('/' and '\') from the specified path.
    /// </summary>
    /// <param name="path">The path to trim.</param>
    /// <returns>The path with trailing separators removed.</returns>
    private static string TrimEndPathSeparator(string path) => path.TrimEnd('\\').TrimEnd('/');
}
