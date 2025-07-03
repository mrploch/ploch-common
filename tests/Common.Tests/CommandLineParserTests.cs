using System.Diagnostics;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Ploch.Common.Tests;

public class CommandLineParserTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(@"C:\test\MyApp\MyApp.exe", @"C:\test\MyApp\MyApp.exe")]
    [InlineData(@"""C:\Program Files\MyApp\MyApp.exe"" -arg1 -arg2", @"C:\Program Files\MyApp\MyApp.exe")]
    [InlineData("\"C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\Binn\\Polybase\\mpdwsvc.exe\" -dms -Polybase",
                "C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\Binn\\Polybase\\mpdwsvc.exe")]
    [InlineData("\"C:\\Program Files\\NVIDIA Corporation\\NvContainer\\nvcontainer.exe\" -s NvContainerLocalSystem -a -f \"C:\\ProgramData\\NVIDIA Corporation\\NVIDIA app\\NvContainer\\NvContainerLocalSystem.log\" -l 3 -d \"C:\\Program Files\\NVIDIA Corporation\\NvContainer\\plugins\\LocalSystem\" -r -p 30000  -ert",
                "C:\\Program Files\\NVIDIA Corporation\\NvContainer\\nvcontainer.exe")]
    public void GetApplicationPath_should_extract_app_path_from_commandline_string(string commandLine, string expectedPath)
    {
        // Arrange
        // Act
        var result = CommandLineParser.GetApplicationPath(commandLine);

#pragma warning disable CS0219 // Variable is assigned but its value is never used
        var str = "dupa";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

        // Assert
        result.Should().Be(expectedPath);
    }

    [Theory]
    [InlineData(@"C:\test\MyApp\MyApp.exe", @"C:\test\MyApp\MyApp.exe", new string[0])]
    [InlineData(@"""C:\Program Files\MyApp\MyApp.exe"" -arg1 -arg2", @"C:\Program Files\MyApp\MyApp.exe", new[] { "-arg1", "-arg2" })]
    [InlineData("\"C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\Binn\\Polybase\\mpdwsvc.exe\" -dms -Polybase",
                "C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\Binn\\Polybase\\mpdwsvc.exe",
                new[] { "-dms", "-Polybase" })]

    //"C:\Program Files\NVIDIA Corporation\NvContainer\nvcontainer.exe" -s NvContainerLocalSystem -a -f "C:\ProgramData\NVIDIA Corporation\NVIDIA app\NvContainer\NvContainerLocalSystem.log" -l 3 -d "C:\Program Files\NVIDIA Corporation\NvContainer\plugins\LocalSystem" -r -p 30000  -ert
    [InlineData("\"C:\\Program Files\\NVIDIA Corporation\\NvContainer\\nvcontainer.exe\" -s NvContainerLocalSystem -a -f \"C:\\ProgramData\\NVIDIA Corporation\\NVIDIA app\\NvContainer\\NvContainerLocalSystem.log\" -l 3 -d \"C:\\Program Files\\NVIDIA Corporation\\NvContainer\\plugins\\LocalSystem\" -r -p 30000 -ert",
                "C:\\Program Files\\NVIDIA Corporation\\NvContainer\\nvcontainer.exe",
                new[]
                { "-s", "NvContainerLocalSystem", "-a", "-f", "C:\\ProgramData\\NVIDIA Corporation\\NVIDIA app\\NvContainer\\NvContainerLocalSystem.log", "-l",
                  "3", "-d", "C:\\Program Files\\NVIDIA Corporation\\NvContainer\\plugins\\LocalSystem", "-r", "-p", "30000", "-ert" })]
    public void GetCommandLine_should_extract_app_path_from_commandline_string(string commandLine, string expectedPath, string[] arguments)
    {
        // Arrange
        // Act
        var result = CommandLineParser.GetCommandLine(commandLine);

        // Assert
        result?.ApplicationPath.Should().Be(expectedPath);
        result.HasValue.Should().BeTrue();
        result!.Value.Arguments.Should().BeEquivalentTo(arguments);
    }

    [Fact]
    public void GetApplicationPath_should_allow_FileVersionInfor_to_be_extracted()
    {
        var exceptionCount = 0;
        var successCount = 0;
        foreach (var commandLine in File.ReadAllLines("commandLines.txt"))
        {
            var s = "\"\"";
            var applicationPath = CommandLineParser.GetApplicationPath(commandLine);
            output.WriteLine(applicationPath);
            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(applicationPath);
                output.WriteLine(fileVersionInfo.ToString());
                fileVersionInfo.FileName.Should().Be(applicationPath);

                successCount++;
            }
            catch (FileNotFoundException ex)
            {
                exceptionCount++;
                output.WriteLine($"File not found: {ex.Message}, exception count: {exceptionCount}");
            }
        }

        successCount.Should().BeGreaterThan(150);
        exceptionCount.Should().BeLessThan(30);
    }

    [Fact]
    public void GetAsArguments_should_return_parts_as_arguments_only_instead_of_application_and_arguments()
    {
        var commandLine = "--id 2 --name Game";

        var actual = CommandLineParser.GetAsArguments(commandLine);

        actual.Should().NotBeNull();
        actual.Value.ApplicationPath.Should().BeNullOrEmpty();
        actual.Value.Arguments.Should().BeEquivalentTo("--id", "2", "--name", "Game");
    }
}
