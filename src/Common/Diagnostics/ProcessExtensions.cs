using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ploch.Common.ArgumentChecking;

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
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="processorNumber"/> is out of range.</exception>
    /// <remarks>Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.</remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static void SetSingleProcessorAffinity(this Process process, int processorNumber)
    {
        process.NotNull(nameof(process));

        if (processorNumber < 0 || processorNumber >= Environment.ProcessorCount)
        {
            throw new ArgumentOutOfRangeException(nameof(processorNumber), "Processor number must be within valid range.");
        }

        var affinityMask = 1L << processorNumber;
#pragma warning disable CA2020 // Unchecked is intentional: the affinity bitmask's high bit (e.g. processor 31 on a 32-bit process) must wrap to the native pointer pattern, not throw OverflowException.
        process.ProcessorAffinity = unchecked((IntPtr)affinityMask);
#pragma warning restore CA2020
    }

    /// <summary>
    /// Sets the processor affinity of the process to the specified set of processors.
    /// </summary>
    /// <param name="process">The process whose affinity will be set.</param>
    /// <param name="enabledProcessorsNumbers">An array of zero-based processor numbers to enable.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if no processor numbers are specified.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if any processor number is out of range.</exception>
    /// <remarks>Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.</remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static void SetEnabledProcessors(this Process process, params int[] enabledProcessorsNumbers)
    {
        process.NotNull(nameof(process));

        if (enabledProcessorsNumbers == null || enabledProcessorsNumbers.Length == 0)
        {
            throw new ArgumentException("At least one processor number must be specified.", nameof(enabledProcessorsNumbers));
        }

        var processorCount = Environment.ProcessorCount;
        long affinityMask = 0;
        foreach (var number in enabledProcessorsNumbers)
        {
            if (number < 0 || number >= processorCount)
            {
                throw new ArgumentOutOfRangeException(nameof(enabledProcessorsNumbers), $"Processor number {number} is out of range.");
            }

            affinityMask |= 1L << number;
        }

#pragma warning disable CA2020 // Unchecked is intentional: the affinity bitmask's high bit (e.g. processor 31 on a 32-bit process) must wrap to the native pointer pattern, not throw OverflowException.
        process.ProcessorAffinity = unchecked((IntPtr)affinityMask);
#pragma warning restore CA2020
    }

    /// <summary>
    /// Gets the list of enabled processor numbers for the process based on its current affinity mask.
    /// </summary>
    /// <param name="process">The process to query.</param>
    /// <returns>An enumerable of zero-based processor numbers that are enabled for the process.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is <see langword="null"/>.</exception>
    /// <remarks>Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.</remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static IEnumerable<int> GetEnabledProcessors(this Process process)
    {
        process.NotNull(nameof(process));

        // Materialise eagerly (rather than a yield iterator) so the argument guard above throws at
        // call time instead of being deferred until the sequence is enumerated.
        var affinityMask = process.ProcessorAffinity.ToInt64();
        var enabledProcessors = new List<int>();
        for (var i = 0; i < Environment.ProcessorCount; i++)
        {
            if ((affinityMask & (1L << i)) != 0)
            {
                enabledProcessors.Add(i);
            }
        }

        return enabledProcessors;
    }
}
