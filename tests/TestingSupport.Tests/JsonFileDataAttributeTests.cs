using FluentAssertions;
using Ploch.TestingSupport.Tests.Models;

namespace Ploch.TestingSupport.Tests;

public class JsonFileDataAttributeTests
{
    [Theory]
    [JsonFileData("TestData.json", "Student")]
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

    [Theory]
    [JsonFileData("TestData.json", "Group")]
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
}
