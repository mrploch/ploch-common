using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AutoFixture;

namespace Ploch.TestingSupport.FileSystem
{
    public class DirectoryStructureInfo
    {
        public DirectoryStructureInfo(IDirectoryInfo directory)
        {
            Directory = directory;
        }

        public IDirectoryInfo Directory { get; }
        public IList<IFileInfo> Files { get; } = new List<IFileInfo>();

        public IList<DirectoryStructureInfo> Directories { get; } = new List<DirectoryStructureInfo>();
    }

    public enum FileType
    {
        PlainText,
        RandomBinary,
        Custom
    }

    public class FileStructureGeneratorConfiguration
    {
        private const string FileNamePrefixToken = "{fileNamePrefix}";
        private const string FileNumberToken = "{fileNumber}";
        private const string FileExtensionToken = "{fileExtension}";

        private const string DirectoryNamePrefixToken = "{directoryNamePrefix}";
        private const string DirectoryNumberToken = "{directoryNumber}";

        private const string DefaultFileNamePrefix = "file_";
        private const string DefaultFileExtension = "txt";
        private const string DefaultDirectoryNamePrefix = "dir_";
        private const string DefaultFileNamePattern = "{fileNamePrefix}{fileNumber}.{fileExtension}";
        private const string DefaultDirectoryNamePattern = "{directoryNamePrefix}{directoryNumber}";

        public FileStructureGeneratorConfiguration(string fileNamePrefix = DefaultDirectoryNamePrefix,
                                                   string fileNamePattern = DefaultFileNamePattern,
                                                   string directoryNamePrefix = DefaultDirectoryNamePrefix,
                                                   string directoryNamePattern = DefaultDirectoryNamePattern,
                                                   params string[] fileExtensions)
        {
            FileNamePrefix = fileNamePrefix;
            FileExtensions = fileExtensions;
            FileNamePattern = fileNamePattern;
            DirectoryNamePrefix = directoryNamePrefix;
            DirectoryNamePattern = directoryNamePattern;
        }

        public string FileNamePrefix { get; set; }

        public ICollection<string> FileExtensions { get; set; }

        public string FileNamePattern { get; set; }

        public string DirectoryNamePrefix { get; set; }

        public string DirectoryNamePattern { get; set; }

        public string GetFileName(int fileNumber, string fileExtension)
        {
            return GetFileName(FileNamePrefix, fileNumber, fileExtension);
        }

        public string GetFileName(string fileNamePrefix, int fileNumber, string fileExtension)
        {
            return FileNamePattern.Replace(FileNamePrefixToken, fileNamePrefix)
                                  .Replace(FileNumberToken, fileNumber.ToString())
                                  .Replace(FileExtensionToken, fileExtension);
        }

        public string GetDirectoryName(int directoryNumber)
        {
            return GetDirectoryName(DirectoryNamePrefix, directoryNumber);
        }

        public string GetDirectoryName(string directoryNamePrefix, int directoryNumber)
        {
            return DirectoryNamePattern.Replace(DirectoryNamePrefixToken, directoryNamePrefix).Replace(DirectoryNumberToken, directoryNumber.ToString());
        }
    }

    public interface IFileTypeCreator
    {
        /// <summary>
        /// If an out-of-the box file type, this property will contain the type from <see cref="FileType"/> enumeration.
        /// When implementing a custom <see cref="IFileTypeCreator"/> using this library and no <see cref="FileType"/>
        /// exists, use <c>FileType.Custom</c>
        /// </summary>
        FileType FileType { get; }

        /// <summary>
        /// Default extension for this file type.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Creates a file and writes the content using information from <paramref name="fileInfo"/>.
        /// Usually, a file represented by <paramref name="fileInfo"/> will not exist and this method will create it.
        /// If a file exists, then behaviour will depend on the implementation.
        /// </summary>
        /// <param name="fileInfo">Represents the information about a file that will be created.</param>
        void Create(IFileInfo fileInfo);
    }

    /// <summary>
    /// Interface identifying types that use <see href="https://github.com/AutoFixture/AutoFixture">AutoFixture</see> library to create
    /// anonymous types and values. Such types can then be configured in a common way.
    /// </summary>
    public interface IUsingFixture
    {
        IFixture Fixture { get; set; }
    }

    /// <summary>
    /// Implementation of <see cref="IFileTypeCreator"/> that creates text files.
    /// </summary>
    public class TextFileTypeCreator : IFileTypeCreator, IUsingFixture
    {
        private readonly bool _randomContent;
        private readonly int _contentLength;
        private readonly Func<string> _contentProvider;

        /// <inheritdoc />
        public FileType FileType => FileType.PlainText;

        public IFixture Fixture { get; set; } = new Fixture();


        /// <inheritdoc />
        public string Extension => "txt";

        public TextFileTypeCreator(bool randomContent = true, int contentLength = -1, Func<string> contentProvider = null)
        {
            _randomContent = randomContent;
            _contentLength = contentLength;
            _contentProvider = contentProvider;
        }

        /// <inheritdoc />
        public void Create(IFileInfo fileInfo)
        {
            using var writer = fileInfo.CreateText();

            writer.Write(Fixture.Create(GetType().Name));
        }
    }

    public class TestFileStructureGenerator
    {
        private readonly IFileSystem _fileSystem;
        private readonly FileStructureGeneratorConfiguration _configuration;

        public TestFileStructureGenerator(IFileSystem fileSystem, FileStructureGeneratorConfiguration configuration)
        {
            _fileSystem = fileSystem;
            _configuration = configuration;
        }

        public void CreateDirectoryStructure(string rootPath,
                                             string namePrefix,
                                             int levels = 3,
                                             int foldersPerLevel = 3,
                                             int filesPerFolder = 3)
        {
            var rootDir = _fileSystem.DirectoryInfo.FromDirectoryName(rootPath);
        }

        public DirectoryStructureInfo CreateTestFiles(IDirectoryInfo directory,
                                                      int fileCount = 3,
                                                      Func<IDirectoryInfo, string, string, IFileInfo> createFileFunc = null,
                                                      DirectoryStructureInfo parentInfo = null)
        {
            parentInfo ??= new DirectoryStructureInfo(directory);
            createFileFunc ??= CreateFile;
            for (var i = 0; i < fileCount; i++)
            {
                //var file = createFileFunc(directory, )
            }

            return parentInfo;
        }

        public IFileInfo CreateFile(IDirectoryInfo directory,
                                    string name = null,
                                    string extension = null)
        {
            var fixture = new Fixture();
            name ??= _configuration.FileNamePrefix + fixture.Create<string>();
            extension ??= _configuration.FileExtensions.First();

            var file = _fileSystem.FileInfo.FromFileName(Path.Combine(directory.FullName, $"{name}.{extension}"));
            
            using var writer = file.CreateText();
            writer.WriteLine(fixture.Create<string>());
            return file;
        }

        public DirectoryStructureInfo CreateDirectoryStructure(IDirectoryInfo rootDir,
                                                               string directoryNamePrefix = null,
                                                               int levels = 3,
                                                               int foldersPerLevel = 3,
                                                               Action<IDirectoryInfo, DirectoryStructureInfo> directoryCreatedAction = null,
                                                               DirectoryStructureInfo parentInfo = null)
        {
            if (levels < 0) return null;

            if (!rootDir.Exists) rootDir.Create();

            var rootDirInfo = new DirectoryStructureInfo(rootDir);
            parentInfo?.Directories.Add(rootDirInfo);

            directoryCreatedAction?.Invoke(rootDir, rootDirInfo);
            directoryNamePrefix ??= _configuration.DirectoryNamePrefix;
            for (var i = 0; i < foldersPerLevel; i++)
            {
                var subdirectoryName = _configuration.GetDirectoryName(directoryNamePrefix, i);
                var subdirectory = rootDir.CreateSubdirectory(subdirectoryName);
                var directoryStructure = CreateDirectoryStructure(subdirectory, subdirectoryName, levels - 1, foldersPerLevel, directoryCreatedAction);
                rootDirInfo.Directories.Add(directoryStructure);
            }

            return rootDirInfo;
        }
    }
}