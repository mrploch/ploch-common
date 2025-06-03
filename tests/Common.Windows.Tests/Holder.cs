using Xunit.Abstractions;

namespace Ploch.Common.Windows.Tests;

public static class Holder
{
    public static ITestOutputHelper Output { get; set; } = null!;

    public static IDictionary<string, ServiceInfo> AllServices { get; set; } = null!;

    public static DebugTool DebugTool { get; } = new();
}