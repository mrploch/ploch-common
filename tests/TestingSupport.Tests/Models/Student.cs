namespace Ploch.TestingSupport.Tests.Models;

public class Student
{
    public Student()
    {
    }

    public Student(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int Age { get; set; }
}
