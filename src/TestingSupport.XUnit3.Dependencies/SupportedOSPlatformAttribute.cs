using System.Reflection;
using System.Runtime.InteropServices;
using Xunit.v3;

namespace Ploch.TestingSupport.XUnit3.Dependencies;

/// <summary>
///     The <c>SupportedOSAttribute</c> is a custom attribute that specifies which operating systems
///     are supported for a particular unit test method. It dynamically skips the execution of the
///     test when it is not executed on one of the specified operating systems.
/// </summary>
/// <remarks>
///     This attribute is designed to be used in conjunction with XUnit testing framework.
///     It allows test authors to define platform-specific tests, ensuring that tests will only
///     run on the intended OS(es) and be dynamically skipped otherwise with an appropriate message.
///     Skipped tests will indicate the unsupported OS environment in their skip message.
/// </remarks>
/// <example>
///     This example demonstrates how to decorate a test method to specify supported operating systems:
///     <code>
/// [Fact]
/// [SupportedOS(SupportedOS.Linux, SupportedOS.macOS)]
/// public void MyLinuxMacOSTest()
/// {
/// // Test logic here.
/// }
/// </code>
///     If this test is executed on Windows or FreeBSD, it will be dynamically skipped with a
///     message indicating the current OS is not supported.
/// </example>
public sealed class SupportedOSPlatformAttribute(params SupportedOS[] supportedOSes) :
    BeforeAfterTestAttribute
{
    private static readonly Dictionary<SupportedOS, OSPlatform> osMappings = new()
                                                                             { { SupportedOS.FreeBSD, OSPlatform.Create("FreeBSD") },
                                                                               { SupportedOS.Linux, OSPlatform.Linux },
                                                                               { SupportedOS.macOS, OSPlatform.OSX },
                                                                               { SupportedOS.Windows, OSPlatform.Windows } };

    /// <inheritdoc />
    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        var match = false;

        foreach (var supportedOS in supportedOSes)
        {
            if (!osMappings.TryGetValue(supportedOS, out var osPlatform))
            {
                throw new ArgumentException($"Supported OS value '{supportedOS}' is not a known OS", nameof(supportedOSes));
            }

            if (RuntimeInformation.IsOSPlatform(osPlatform))
            {
                match = true;

                break;
            }
        }

        // We use the dynamic skip exception message pattern to turn this into a skipped test
        // when it's not running on one of the targeted OSes
        if (!match)
        {
            throw new($"$XunitDynamicSkip$This test is not supported on {RuntimeInformation.OSDescription}");
        }
    }
}
