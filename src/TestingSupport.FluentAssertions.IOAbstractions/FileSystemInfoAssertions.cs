using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace TestingSupport.FluentAssertions.IOAbstractions
{
    public class FileSystemInfoAssertions<TItems> : GenericCollectionAssertions<TItems> where TItems: IFileSystemInfo
    {
        protected override string Identifier => nameof(IFileSystemInfo);

        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(params string[] fileSystemInfoNames)
        {
            return HaveNamesEquivalentToIgnoringCase(fileSystemInfoNames);
        }

        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentToIgnoringCase(IEnumerable<string> fileSystemInfoNames,
                                                                                                 string because = "",
                                                                                                 params object[] becauseArgs)
        {
            return HaveNamesEquivalentTo(fileSystemInfoNames, StringComparer.OrdinalIgnoreCase, because, becauseArgs);
        }
        
        public AndConstraint<FileSystemInfoAssertions<TItems>> HaveNamesEquivalentTo(IEnumerable<string> fileSystemInfoNames,
                                                                                     StringComparer equalityComparer,
                                                                                     string because = "",
                                                                                     params object[] becauseArgs)
        {
            Subject.Select(fsi => fsi.Name.TrimEnd('\\'))
                   .OrderBy(name => name)
                   .Should()
                   .BeEquivalentTo(fileSystemInfoNames.Select(name => name.TrimEnd('\\')).OrderBy(name => name), config => config.Using(equalityComparer));
            return new AndConstraint<FileSystemInfoAssertions<TItems>>(this);
        }

        public FileSystemInfoAssertions(IEnumerable<TItems> actualValue) : base(actualValue)
        {
        }
    }
}