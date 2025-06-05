using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;

namespace TestingSupport.FluentAssertions.IOAbstractions
{
    public class FileSystemInfoAssertions<TItems> : GenericCollectionAssertions<TItems>
        where TItems : IFileSystemInfo
    {
        public FileSystemInfoAssertions(IEnumerable<TItems> actualValue) : base(actualValue)
        { }

        protected override string Identifier => nameof(IFileSystemInfo);

        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(params string[] fileSystemInfoNames) =>
            HaveNamesEquivalentToIgnoringCase(fileSystemInfoNames.AsEnumerable());

        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(IEnumerable<string> fileSystemInfoNames,
                                                                                                 string because = "",
                                                                                                 params object[] becauseArgs) =>
            HaveNamesEquivalentTo(fileSystemInfoNames, StringComparer.OrdinalIgnoreCase, because, becauseArgs);

        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentTo(IEnumerable<string> fileSystemInfoNames,
                                                                                     StringComparer equalityComparer,
                                                                                     string because = "",
                                                                                     params object[] becauseArgs)
        {
            Subject.Select(fsi => TrimEndPathSeparator(fsi.Name))
                   .OrderBy(name => name)
                   .Should()
                   .BeEquivalentTo(fileSystemInfoNames.Select(TrimEndPathSeparator).OrderBy(static name => name), config => config.Using(equalityComparer));

            return new AndConstraint<FileSystemInfoAssertions<TItems>>(this);
        }

        private static string TrimEndPathSeparator(string path) => path.TrimEnd('\\').TrimEnd('/');
    }
}
