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
    public void SetSingleProcessorAffinity_should_throw_for_processor_number_exceeding_system_processor_count()
    {
        var process = new Process();

        // Act
        var act = () => process.SetSingleProcessorAffinity(Environment.ProcessorCount);

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

    [Theory]
    [InlineData(128, 64, 64)] // Machine with >64 logical CPUs, 64-bit process: CPU 64 would wrap the shift count (64 & 63 == 0).
    [InlineData(128, 64, 127)] // Machine with >64 logical CPUs, 64-bit process: CPU 127 would wrap to bit 63.
    [InlineData(64, 32, 32)] // 32-bit process on a 64-CPU machine: CPU 32 would be truncated out of the 32-bit mask.
    public void ValidateProcessorNumber_should_throw_when_processor_number_exceeds_affinity_mask_width(int processorCount, int affinityMaskWidth, int processorNumber)
    {
        var act = () => ProcessExtensions.ValidateProcessorNumber(processorNumber, processorCount, affinityMaskWidth, nameof(processorNumber));

        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .Which.Should()
           .Match<ArgumentOutOfRangeException>(exception => Equals(exception.ActualValue, processorNumber) && exception.ParamName == nameof(processorNumber));
    }

    [Theory]
    [InlineData(128, 64, 63)] // Highest bit representable in a 64-bit mask.
    [InlineData(64, 32, 31)] // Highest bit representable in a 32-bit mask.
    [InlineData(4, 64, 3)] // Highest processor when the processor count is the limiting factor.
    [InlineData(4, 64, 0)]
    public void ValidateProcessorNumber_should_accept_processor_number_within_processor_count_and_mask_width(int processorCount, int affinityMaskWidth, int processorNumber)
    {
        var act = () => ProcessExtensions.ValidateProcessorNumber(processorNumber, processorCount, affinityMaskWidth, nameof(processorNumber));

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(128, 64, 64)] // Machine with >64 logical CPUs, 64-bit process: only the first 64 CPUs are addressable.
    [InlineData(64, 32, 32)] // 32-bit process on a 64-CPU machine: only the first 32 CPUs are addressable.
    [InlineData(8, 64, 8)] // Typical machine: the processor count is the limiting factor.
    public void GetMaxAddressableProcessors_should_cap_at_the_lesser_of_processor_count_and_mask_width(int processorCount, int affinityMaskWidth, int expected)
    {
        ProcessExtensions.GetMaxAddressableProcessors(processorCount, affinityMaskWidth).Should().Be(expected);
    }
}
