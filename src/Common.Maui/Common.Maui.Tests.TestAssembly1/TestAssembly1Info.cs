using System.Reflection;

namespace Ploch.Common.Tests.TestAssembly1;

public static class TestAssembly1Info
{
    public static Assembly Assembly
    {
        get => typeof(TestAssembly1Info).Assembly;
    }
}
