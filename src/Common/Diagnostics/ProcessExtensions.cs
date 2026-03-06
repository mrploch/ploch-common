using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ploch.Common.Diagnostics;

/// <summary>
/// Provides extension methods for the <see cref="Process"/> class to manage processor affinity and query enabled processors.
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    /// Sets the processor affinity of the process to a single processor specified by <paramref name="processorNumber"/>.
    /// </summary>
    /// <param name="process">The process whose affinity will be set.</param>
    /// <param name="processorNumber">The zero-based processor number to set affinity to.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="processorNumber"/> is out of range.</exception>
    public static void SetSingleProcessorAffinity(this Process process, int processorNumber)
    {
        // implement with ai
        if (processorNumber < 0 || processorNumber >= Environment.ProcessorCount)
        {
            throw new ArgumentOutOfRangeException(nameof(processorNumber), "Processor number must be within valid range.");
        }

        var affinityMask = 1L << processorNumber;
        process.ProcessorAffinity = (IntPtr)affinityMask;
    }

    /// <summary>
    /// Sets the processor affinity of the process to the specified set of processors.
    /// </summary>
    /// <param name="process">The process whose affinity will be set.</param>
    /// <param name="enabledProcessorsNumbers">An array of zero-based processor numbers to enable.</param>
    /// <exception cref="ArgumentException">Thrown if no processor numbers are specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any processor number is out of range.</exception>
    public static void SetEnabledProcessors(this Process process, params int[] enabledProcessorsNumbers)
    {
        if (enabledProcessorsNumbers == null || enabledProcessorsNumbers.Length == 0)
        {
            throw new ArgumentException("At least one processor number must be specified.", nameof(enabledProcessorsNumbers));
        }

        var processorCount = Environment.ProcessorCount;
        foreach (var number in enabledProcessorsNumbers)
        {
            if (number < 0 || number >= processorCount)
            {
                throw new ArgumentOutOfRangeException(nameof(enabledProcessorsNumbers), $"Processor number {number} is out of range.");
            }
        }

        long affinityMask = 0;
        foreach (var number in enabledProcessorsNumbers)
        {
            affinityMask |= 1L << number;
        }

        process.ProcessorAffinity = (IntPtr)affinityMask;
    }

    /// <summary>
    /// Gets the list of enabled processor numbers for the process based on its current affinity mask.
    /// </summary>
    /// <param name="process">The process to query.</param>
    /// <returns>An enumerable of zero-based processor numbers that are enabled for the process.</returns>
    public static IEnumerable<int> GetEnabledProcessors(this Process process)
    {
        var affinityMask = process.ProcessorAffinity.ToInt64();
        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            if ((affinityMask & (1L << i)) != 0)
            {
                yield return i;
            }
        }
    }
}
