using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Ploch.TestingSupport.TestData;
using Ploch.TestingSupport.Tests.Models;
using Xunit;
using Xunit.Sdk;

namespace Ploch.TestingSupport.Tests;
#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes
public class JsonFileDataAttributeTests
{
  private static readonly MethodInfo TestMethodInfo = typeof(JsonFileDataAttributeTests)
    .GetMethod(nameof(EnrollStudent_Success), BindingFlags.Public | BindingFlags.Instance)!;

  [Theory, JsonFileData("TestData/TestData.json", "Student")] 
  public void EnrollStudent_Success(Student student, int id)
  {
    if (id == 0)
    {
      student.FirstName.Should().Be("Dane");
      student.LastName.Should().Be("Gates");
      student.Age.Should().Be(34);
    }

    if (id == 1)
    {
      student.FirstName.Should().Be("Sidney");
      student.LastName.Should().Be("Graham");
      student.Age.Should().Be(27);
    }

    student.Should().NotBeNull();
  }

  [Theory, JsonFileData("TestData/TestData.json", "Group")] 
  public void EnrollGroup_Success(List<Student> students, int id)
  {
    if (id == 0)
    {
      students.Should().HaveCount(6);
      students[0].FirstName.Should().Be("Bethany");
      students[0].LastName.Should().Be("Lawson");
      students[0].Age.Should().Be(29);
      students[1].FirstName.Should().Be("Kayden");
      students[1].LastName.Should().Be("Hamilton");
      students[1].Age.Should().Be(20);
      students[2].FirstName.Should().Be("Ash");
      students[2].LastName.Should().Be("Mason");
      students[2].Age.Should().Be(22);
      students[3].FirstName.Should().Be("Alisha");
      students[3].LastName.Should().Be("Richardson");
      students[3].Age.Should().Be(30);
      students[4].FirstName.Should().Be("Erin");
      students[4].LastName.Should().Be("Schneider");
      students[4].Age.Should().Be(50);
      students[5].FirstName.Should().Be("Tyler");
      students[5].LastName.Should().Be("Sandoval");
      students[5].Age.Should().Be(45);
      students.Should().NotBeNull();
    }

    if (id == 1)
    {
      students.Should().HaveCount(4);
      students[0].FirstName.Should().Be("George");
      students[0].LastName.Should().Be("Gibson");
      students[0].Age.Should().Be(21);
      students[1].FirstName.Should().Be("Lindsay");
      students[1].LastName.Should().Be("Blair");
      students[1].Age.Should().Be(28);
      students[2].FirstName.Should().Be("George");
      students[2].LastName.Should().Be("Gibson");
      students[2].Age.Should().Be(21);
      students[3].FirstName.Should().Be("Lindsay");
      students[3].LastName.Should().Be("Blair");
      students[3].Age.Should().Be(28);
      students.Should().NotBeNull();
    }

    students.Should().NotBeNull();
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public async Task GetData_should_throw_when_filePath_is_null_or_whitespace(string? invalidFilePath)
  {
    var attribute = new JsonFileDataAttribute(invalidFilePath ?? string.Empty, "Student");

    Func<Task> act = () => InvokeGetDataAsync(attribute, TestMethodInfo);

    var exception = await act.Should().ThrowAsync<ArgumentException>();
    exception.Which.ParamName.Should().Be("filePath");
  }

  [Fact]
  public async Task GetData_should_throw_when_property_is_missing()
  {
    await WithTempFileAsync("{ \"Existing\": [] }", async tempFile =>
    {
      var attribute = new JsonFileDataAttribute(tempFile, "MissingProperty");
      Func<Task> act = () => InvokeGetDataAsync(attribute, TestMethodInfo);

      var exception = await act.Should().ThrowAsync<ArgumentException>();
      exception.Which.ParamName.Should().Be("propertyName");
    });
  }

  [Theory]
  [InlineData("Student")]
  [InlineData("Group")]
  public async Task GetData_should_throw_when_property_is_not_array(string propertyName)
  {
    await WithTempFileAsync("{ \"Student\": { \"FirstName\": \"John\" }, \"Group\": { \"Key\": 1 } }", async tempFile =>
    {
      var attribute = new JsonFileDataAttribute(tempFile, propertyName);
      Func<Task> act = () => InvokeGetDataAsync(attribute, TestMethodInfo);

      var exception = await act.Should().ThrowAsync<ArgumentException>();
      exception.Which.ParamName.Should().Be("propertyName");
    });
  }

  private static async Task InvokeGetDataAsync(JsonFileDataAttribute attribute, MethodInfo methodInfo)
  {
    // DisposalTracker is required by the xUnit v3 data source contract to capture disposable resources.
    await using var disposalTracker = new DisposalTracker();
    await attribute.GetData(methodInfo, disposalTracker).AsTask();
  }

  private static async Task WithTempFileAsync(string content, Func<string, Task> action)
  {
    var tempFile = Path.GetTempFileName();
    await File.WriteAllTextAsync(tempFile, content);

    try
    {
      await action(tempFile);
    }
    finally
    {
      try
      {
        if (File.Exists(tempFile))
        {
          File.Delete(tempFile);
        }
      }
      catch (IOException)
      {
        // best-effort cleanup for test temp files
      }
      catch (UnauthorizedAccessException)
      {
        // best-effort cleanup for test temp files
      }
    }
  }
}
#pragma warning restore xUnit1003
