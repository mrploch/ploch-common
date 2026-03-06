using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Xunit.Abstractions;

namespace TestingSupport.FluentAssertions.IOAbstractions.Tests;

public class FileSystemInfoAssertionsTests(ITestOutputHelper output)
{
    [Fact]
    public void HaveNamesEquivalentTo_should_compare_names_only_and_not_fail_if_the_match()
    {
        var fileSystemInfoNames = new List<string> { "file1.txt", "file2.txt", "file3.txt", "folder1_1", "folder1_2" };
        var fileSystemInfoNames2 = new List<string> { "file1.txt", "file2.txt", "file3.txt", "folder1_1", "folder1_2" };

        var fileSystem = BuildFileSystem();

        var fileSystemInfos = fileSystem.DirectoryInfo.New("c:/folder1").GetFileSystemInfos();

        fileSystemInfos.Should().HaveNamesEquivalentTo(fileSystemInfoNames, StringComparer.Ordinal);
    }

    [Fact]
    public void HaveNamesEquivalentTo_should_compare_names_only_and_fail_if_they_dont_match_count_different()
    {
        var fileSystem = BuildFileSystem();

        output.WriteLine($"FileSystem PathSeparator: {fileSystem.Path.PathSeparator}");
        output.WriteLine($"FileSystem DirectorySeparatorChar: {fileSystem.Path.DirectorySeparatorChar}");
        output.WriteLine($"FileSystem DirectorySeparatorChar: {fileSystem.Path.VolumeSeparatorChar}");
        var fileSystemInfos = fileSystem.DirectoryInfo.New("c:/folder1").GetFileSystemInfos();

        fileSystemInfos.Should().HaveNamesEquivalentTo(new[] { "file1.txt", "file2.txt", "file3.txt", "folder1_1", "folder1_2" }, StringComparer.Ordinal);
    }

    private static IFileSystem BuildFileSystem()
    {
        var fileSystem = new MockFileSystem();

        fileSystem.AddDirectory("c:/folder1");
        fileSystem.AddFile("c:/folder1/file1.txt", new MockFileData(Guid.NewGuid().ToString()));
        fileSystem.AddFile("c:/folder1/file2.txt", new MockFileData(Guid.NewGuid().ToString()));
        fileSystem.AddFile("c:/folder1/file3.txt", new MockFileData(Guid.NewGuid().ToString()));
        fileSystem.AddDirectory("c:/folder1/folder1_1");
        fileSystem.AddDirectory("c:/folder1/folder1_2");

        return fileSystem;
    }
}
