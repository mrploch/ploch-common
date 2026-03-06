using System.Diagnostics;
using Ploch.TestingSupport.XUnit3;

namespace Ploch.Common.Diagnostics.Tests;

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

        // Environment.ProcessorCount
        // process.Aff

        // // Arrange
        // var processMock = new Mock<Process>();
        // var processorNumber = 2;
        // var expectedMask = 1L << processorNumber;
        // processMock.SetupProperty(p => p.ProcessorAffinity);
        //
        // // Act
        // processMock.Object.SetSingleProcessorAffinity(processorNumber);
        //
        // // Assert
        // processMock.Object.ProcessorAffinity.Should().Be((IntPtr)expectedMask);
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
}
