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

    [Fact]
    public void GetEnabledProcessors_should_throw_for_null_process()
    {
        var act = () => ((Process)null!).GetEnabledProcessors();

        act.Should().Throw<ArgumentNullException>();
    }
}
