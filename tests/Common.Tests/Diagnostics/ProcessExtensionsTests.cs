using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Ploch.TestingSupport.XUnit3;

namespace Ploch.Common.Diagnostics.Tests;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "These tests deliberately exercise the Windows/Linux-only processor-affinity APIs; the custom [SupportedOSPlatform(SupportedOS.Windows)] trait skips them on unsupported platforms at run time.")]
public class ProcessExtensionsTests
{
    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void SetSingleProcessorAffinity_should_set_affinity_mask_for_valid_processor_number()
    {
        var process = Process.Start("../../../../../src/TestingSupport.MockConsoleApp/bin/Debug/net10.0/Ploch.TestingSupport.MockConsoleApp.exe");

        var enabledProcessors = process.GetEnabledProcessors();
        enabledProcessors.Should().HaveCount(Environment.ProcessorCount);

        process.SetSingleProcessorAffinity(Environment.ProcessorCount - 1);

        enabledProcessors = process.GetEnabledProcessors();
        enabledProcessors.Should().HaveCount(1);
        enabledProcessors.Should().Contain(Environment.ProcessorCount - 1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void SetSingleProcessorAffinity_should_throw_for_invalid_processor_number(int processorNumber)
    {
        // Arrange
        var process = new Process();

        // Act
        var act = () => process.SetSingleProcessorAffinity(processorNumber);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetSingleProcessorAffinity_should_throw_for_processor_number_beyond_native_mask_width()
    {
        var process = new Process();

        // IntPtr.Size * 8 is the first processor number that cannot be represented in the native affinity mask.
        var act = () => process.SetSingleProcessorAffinity(IntPtr.Size * 8);

        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("processorNumber");
    }

    [Fact]
    public void SetSingleProcessorAffinity_should_throw_for_null_process()
    {
        var act = () => ((Process)null!).SetSingleProcessorAffinity(0);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetEnabledProcessors_should_throw_for_null_process()
    {
        var act = () => ((Process)null!).SetEnabledProcessors(0);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SetEnabledProcessors_should_throw_when_no_processor_numbers_are_specified()
    {
        var process = new Process();

        var act = () => process.SetEnabledProcessors();

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void SetEnabledProcessors_should_throw_for_invalid_processor_number(int processorNumber)
    {
        var process = new Process();

        var act = () => process.SetEnabledProcessors(processorNumber);

        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("enabledProcessorsNumbers");
    }

    [Fact]
    public void SetEnabledProcessors_should_throw_for_processor_number_beyond_native_mask_width()
    {
        var process = new Process();

        // IntPtr.Size * 8 is the first processor number that cannot be represented in the native affinity mask.
        var act = () => process.SetEnabledProcessors(0, IntPtr.Size * 8);

        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("enabledProcessorsNumbers");
    }

    [Fact]
    public void GetEnabledProcessors_should_throw_for_null_process()
    {
        var act = () => ((Process)null!).GetEnabledProcessors();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows, SupportedOS.Linux)]
    public void GetEnabledProcessors_should_report_the_processors_set_in_the_native_affinity_mask()
    {
        var process = Process.GetCurrentProcess();

        var enabledProcessors = process.GetEnabledProcessors();

        // Independently extract the expected processor numbers from the OS-reported mask value.
        var expectedProcessors = GetAllowedProcessors(process);
        enabledProcessors.Should().NotBeEmpty().And.Equal(expectedProcessors);
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows, SupportedOS.Linux)]
    public void SetSingleProcessorAffinity_should_enable_only_the_requested_processor()
    {
        // A throwaway child process is mutated (never the test host) so parallel test collections are unaffected.
        using var childProcess = StartShortLivedChildProcess();

        try
        {
            // The target is taken from the OS-reported mask so the test also works when CPU 0 is excluded by a cpuset.
            var targetProcessor = GetAllowedProcessors(childProcess)[0];

            childProcess.SetSingleProcessorAffinity(targetProcessor);

            childProcess.GetEnabledProcessors().Should().Equal(targetProcessor);
        }
        finally
        {
            KillAndReap(childProcess);
        }
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows, SupportedOS.Linux)]
    public void SetEnabledProcessors_should_enable_exactly_the_requested_processors()
    {
        // A throwaway child process is mutated (never the test host) so parallel test collections are unaffected.
        using var childProcess = StartShortLivedChildProcess();

        try
        {
            // Targets are taken from the OS-reported mask so the test also works when CPU 0 is excluded by a cpuset.
            var requestedProcessors = GetAllowedProcessors(childProcess).Take(2).ToArray();

            childProcess.SetEnabledProcessors(requestedProcessors);

            childProcess.GetEnabledProcessors().Should().Equal(requestedProcessors);
        }
        finally
        {
            KillAndReap(childProcess);
        }
    }

    private static Process StartShortLivedChildProcess()
    {
        // Cross-platform idle child: `ping` on Windows, `sleep` on Linux. Killed by the test before it exits on its own.
        var startInfo = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("ping", "-n 30 127.0.0.1")
            : new ProcessStartInfo("sleep", "30");
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;

        return Process.Start(startInfo)!;
    }

    private static int[] GetAllowedProcessors(Process process)
    {
        // Derived from the raw OS-reported mask (not from GetEnabledProcessors) so it stays independent of the code under test.
        var affinityMask = process.ProcessorAffinity.ToInt64();

        return Enumerable.Range(0, IntPtr.Size * 8).Where(processor => (affinityMask & (1L << processor)) != 0).ToArray();
    }

    private static void KillAndReap(Process process)
    {
        // Kill can race with (or follow) a natural exit; the guard keeps cleanup from throwing and WaitForExit reaps the child.
        if (!process.HasExited)
        {
            process.Kill();
        }

        process.WaitForExit();
    }

    [Theory]
    [InlineData(64, 64)] // 64-bit process: CPU 64 would wrap the shift count (64 & 63 == 0).
    [InlineData(64, 127)] // 64-bit process: CPU 127 would wrap to bit 63.
    [InlineData(32, 32)] // 32-bit process: CPU 32 would be truncated out of the 32-bit mask.
    [InlineData(64, -1)] // Negative processor numbers are never valid.
    public void ValidateProcessorNumber_should_throw_when_processor_number_is_not_representable_in_the_mask(int affinityMaskWidth, int processorNumber)
    {
        var act = () => ProcessExtensions.ValidateProcessorNumber(processorNumber, affinityMaskWidth, nameof(processorNumber));

        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .Which.Should()
           .Match<ArgumentOutOfRangeException>(exception => Equals(exception.ActualValue, processorNumber) && exception.ParamName == nameof(processorNumber));
    }

    [Theory]
    [InlineData(64, 63)] // Highest bit representable in a 64-bit mask.
    [InlineData(32, 31)] // Highest bit representable in a 32-bit mask.
    [InlineData(64, 0)]
    public void ValidateProcessorNumber_should_accept_processor_number_within_the_mask_width(int affinityMaskWidth, int processorNumber)
    {
        var act = () => ProcessExtensions.ValidateProcessorNumber(processorNumber, affinityMaskWidth, nameof(processorNumber));

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0xFF00L, 64, new[] { 8, 9, 10, 11, 12, 13, 14, 15 })] // Non-contiguous set: processors 8-15 (e.g. a cpuset-constrained process where ProcessorCount == 8).
    [InlineData(0x1L, 64, new[] { 0 })]
    [InlineData(unchecked((long)0x8000000000000001UL), 64, new[] { 0, 63 })] // Highest and lowest bits of a 64-bit mask.
    public void GetEnabledProcessors_should_extract_all_set_bits_within_the_mask_width(long affinityMask, int affinityMaskWidth, int[] expectedProcessors)
    {
        ProcessExtensions.GetEnabledProcessors(affinityMask, affinityMaskWidth).Should().Equal(expectedProcessors);
    }

    [Fact]
    public void GetEnabledProcessors_should_return_no_processors_for_an_empty_mask()
    {
        ProcessExtensions.GetEnabledProcessors(0L, 64).Should().BeEmpty();
    }

    [Fact]
    public void GetEnabledProcessors_should_ignore_bits_beyond_the_mask_width()
    {
        // Bit 32 is not representable in a 32-bit mask.
        ProcessExtensions.GetEnabledProcessors(0x100000000L, 32).Should().BeEmpty();
    }

    [Theory]
    [InlineData(65)] // The 1L << i shift would wrap for bit positions of 64 and above.
    [InlineData(-1)]
    public void GetEnabledProcessors_should_throw_for_a_mask_width_that_a_64bit_mask_cannot_represent(int affinityMaskWidth)
    {
        var act = () => ProcessExtensions.GetEnabledProcessors(0x1L, affinityMaskWidth);

        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be(nameof(affinityMaskWidth));
    }

    [Theory]
    [InlineData(0x3L, 0x3L)] // Everything requested was applied.
    [InlineData(0x1L, 0x3L)] // The OS may report more processors than requested; only the requested ones matter.
    [InlineData(0x80000000L, unchecked((long)0xFFFFFFFF80000000UL))] // 32-bit process: bit 31 applied, read back sign-extended by IntPtr.ToInt64().
    public void VerifyAppliedAffinity_should_accept_when_every_requested_processor_was_applied(long requestedMask, long appliedMask)
    {
        var act = () => ProcessExtensions.VerifyAppliedAffinity(requestedMask, appliedMask);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(unchecked((long)0x8000000000000001UL), 0x1L, "63")] // Linux silently drops the unavailable CPU 63 (sched_setaffinity intersects).
    [InlineData(0x3L, 0x1L, "1")]
    public void VerifyAppliedAffinity_should_throw_when_a_requested_processor_was_not_applied(long requestedMask, long appliedMask, string expectedMissingProcessor)
    {
        var act = () => ProcessExtensions.VerifyAppliedAffinity(requestedMask, appliedMask);

        act.Should().Throw<InvalidOperationException>().WithMessage($"*{expectedMissingProcessor}*");
    }
}
