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
    /// Gets the width, in bits, of the native <see cref="Process.ProcessorAffinity"/> mask for the current process
    /// (32 in a 32-bit process, 64 in a 64-bit process).
    /// </summary>
    private static int AffinityMaskWidth => IntPtr.Size * 8;

    /// <summary>
    /// Gets the exclusive upper bound for addressable processor numbers: the lesser of
    /// <see cref="Environment.ProcessorCount"/> and <see cref="AffinityMaskWidth"/>.
    /// </summary>
    private static int MaxAddressableProcessors => GetMaxAddressableProcessors(Environment.ProcessorCount, AffinityMaskWidth);

    /// <summary>
    /// Sets the processor affinity of the process to a single processor specified by <paramref name="processorNumber"/>.
    /// </summary>
    /// <param name="process">The process whose affinity will be set.</param>
    /// <param name="processorNumber">The zero-based processor number to set affinity to.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="processorNumber"/> is negative, or not below the lesser of
    /// <see cref="Environment.ProcessorCount"/> and the native affinity-mask width (<c>IntPtr.Size * 8</c>).
    /// </exception>
    /// <remarks>
    /// Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.
    /// <para>
    /// <see cref="Process.ProcessorAffinity"/> is a pointer-sized bitmask, so processors beyond the native pointer width cannot be
    /// addressed — for example processors 32 and above in a 32-bit process, or 64 and above on machines with more than
    /// 64 logical processors. Such processor numbers are rejected with <see cref="ArgumentOutOfRangeException"/>.
    /// </para>
    /// </remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static void SetSingleProcessorAffinity(this Process process, int processorNumber)
    {
        process.NotNull(nameof(process));

        ValidateProcessorNumber(processorNumber, Environment.ProcessorCount, AffinityMaskWidth, nameof(processorNumber));

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
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if any processor number is negative, or not below the lesser of
    /// <see cref="Environment.ProcessorCount"/> and the native affinity-mask width (<c>IntPtr.Size * 8</c>).
    /// </exception>
    /// <remarks>
    /// Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.
    /// <para>
    /// <see cref="Process.ProcessorAffinity"/> is a pointer-sized bitmask, so processors beyond the native pointer width cannot be
    /// addressed — for example processors 32 and above in a 32-bit process, or 64 and above on machines with more than
    /// 64 logical processors. Such processor numbers are rejected with <see cref="ArgumentOutOfRangeException"/>.
    /// </para>
    /// </remarks>
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

        long affinityMask = 0;
        foreach (var number in enabledProcessorsNumbers)
        {
            ValidateProcessorNumber(number, Environment.ProcessorCount, AffinityMaskWidth, nameof(enabledProcessorsNumbers));

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
    /// <remarks>
    /// Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.
    /// <para>
    /// Only processors representable in the native <see cref="Process.ProcessorAffinity"/> bitmask are reported — at most
    /// <c>IntPtr.Size * 8</c> processors (32 in a 32-bit process, 64 in a 64-bit process).
    /// </para>
    /// </remarks>
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
        for (var i = 0; i < MaxAddressableProcessors; i++)
        {
            if ((affinityMask & (1L << i)) != 0)
            {
                enabledProcessors.Add(i);
            }
        }

        return enabledProcessors;
    }

    /// <summary>
    /// Gets the number of processors addressable in the affinity mask: the lesser of
    /// <paramref name="processorCount"/> and <paramref name="affinityMaskWidth"/>.
    /// </summary>
    /// <param name="processorCount">The number of logical processors on the machine.</param>
    /// <param name="affinityMaskWidth">The width, in bits, of the native affinity mask.</param>
    /// <returns>The exclusive upper bound for addressable processor numbers.</returns>
    /// <remarks>
    /// Internal (rather than private) so the mask-width capping can be unit-tested with limits that do not occur on the
    /// test machine — more than 64 logical processors, or the 32-bit-process mask width.
    /// </remarks>
    internal static int GetMaxAddressableProcessors(int processorCount, int affinityMaskWidth) => Math.Min(processorCount, affinityMaskWidth);

    /// <summary>
    /// Validates that <paramref name="processorNumber"/> is addressable within the affinity mask: non-negative and below
    /// the lesser of <paramref name="processorCount"/> and <paramref name="affinityMaskWidth"/>.
    /// </summary>
    /// <param name="processorNumber">The zero-based processor number to validate.</param>
    /// <param name="processorCount">The number of logical processors on the machine.</param>
    /// <param name="affinityMaskWidth">The width, in bits, of the native affinity mask.</param>
    /// <param name="paramName">The caller's parameter name to report in the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="processorNumber"/> is not addressable.</exception>
    /// <remarks>
    /// Internal (rather than private) so the mask-width capping can be unit-tested with limits that do not occur on the
    /// test machine — more than 64 logical processors, or the 32-bit-process mask width.
    /// </remarks>
    internal static void ValidateProcessorNumber(int processorNumber, int processorCount, int affinityMaskWidth, string paramName)
    {
        var maxAddressable = GetMaxAddressableProcessors(processorCount, affinityMaskWidth);
        if (processorNumber < 0 || processorNumber >= maxAddressable)
        {
            throw new ArgumentOutOfRangeException(paramName,
                                                  processorNumber,
                                                  $"Processor number must be between 0 and {maxAddressable - 1} — the lesser of the machine's processor count ({processorCount}) and the native affinity-mask width ({affinityMaskWidth} bits).");
        }
    }
}
