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
    /// (32 in a 32-bit process, 64 in a 64-bit process). This is the exclusive upper bound for addressable
    /// processor numbers.
    /// </summary>
    private static int AffinityMaskWidth => IntPtr.Size * 8;

    /// <summary>
    /// Sets the processor affinity of the process to a single processor specified by <paramref name="processorNumber"/>.
    /// </summary>
    /// <param name="process">The process whose affinity will be set.</param>
    /// <param name="processorNumber">The zero-based processor number to set affinity to.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="processorNumber"/> is negative, or not below the native affinity-mask width
    /// (<c>IntPtr.Size * 8</c> — 32 in a 32-bit process, 64 in a 64-bit process).
    /// </exception>
    /// <remarks>
    /// Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.
    /// <para>
    /// <see cref="Process.ProcessorAffinity"/> is a pointer-sized bitmask, so processors beyond the native pointer width cannot be
    /// addressed — for example processors 32 and above in a 32-bit process, or 64 and above on machines with more than
    /// 64 logical processors. Such processor numbers are rejected with <see cref="ArgumentOutOfRangeException"/>.
    /// </para>
    /// <para>
    /// Processor numbers within the mask width are <b>not</b> validated against the number of processors available to the
    /// current process: <see cref="Environment.ProcessorCount"/> is a processor <i>count</i> (which, since .NET 6, already
    /// reflects the process's own affinity and CPU limits), not an upper bound on valid processor indices — a process
    /// constrained to processors 8–15 may legitimately target processor 12. Requesting a processor that does not exist on
    /// the machine is rejected by the operating system when the affinity is applied (typically as a
    /// <see cref="System.ComponentModel.Win32Exception"/>).
    /// </para>
    /// </remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static void SetSingleProcessorAffinity(this Process process, int processorNumber)
    {
        process.NotNull(nameof(process));

        ValidateProcessorNumber(processorNumber, AffinityMaskWidth, nameof(processorNumber));

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
    /// Thrown if any processor number is negative, or not below the native affinity-mask width
    /// (<c>IntPtr.Size * 8</c> — 32 in a 32-bit process, 64 in a 64-bit process).
    /// </exception>
    /// <remarks>
    /// Processor affinity is only supported on Windows and Linux; calling this on other platforms throws <see cref="PlatformNotSupportedException"/>.
    /// <para>
    /// <see cref="Process.ProcessorAffinity"/> is a pointer-sized bitmask, so processors beyond the native pointer width cannot be
    /// addressed — for example processors 32 and above in a 32-bit process, or 64 and above on machines with more than
    /// 64 logical processors. Such processor numbers are rejected with <see cref="ArgumentOutOfRangeException"/>.
    /// </para>
    /// <para>
    /// Processor numbers within the mask width are <b>not</b> validated against the number of processors available to the
    /// current process: <see cref="Environment.ProcessorCount"/> is a processor <i>count</i> (which, since .NET 6, already
    /// reflects the process's own affinity and CPU limits), not an upper bound on valid processor indices — a process
    /// constrained to processors 8–15 may legitimately target processor 12. Requesting a processor that does not exist on
    /// the machine is rejected by the operating system when the affinity is applied (typically as a
    /// <see cref="System.ComponentModel.Win32Exception"/>).
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
            ValidateProcessorNumber(number, AffinityMaskWidth, nameof(enabledProcessorsNumbers));

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
    /// Every bit set in the native <see cref="Process.ProcessorAffinity"/> bitmask is reported — at most
    /// <c>IntPtr.Size * 8</c> processors (32 in a 32-bit process, 64 in a 64-bit process). The result is deliberately not
    /// limited by <see cref="Environment.ProcessorCount"/>, which is a processor <i>count</i> rather than an index bound:
    /// a process constrained to a non-contiguous processor set (for example processors 8–15) reports exactly those
    /// processor numbers.
    /// </para>
    /// </remarks>
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    [System.Runtime.Versioning.SupportedOSPlatform("linux")]
#endif
    public static IEnumerable<int> GetEnabledProcessors(this Process process)
    {
        process.NotNull(nameof(process));

        return GetEnabledProcessors(process.ProcessorAffinity.ToInt64(), AffinityMaskWidth);
    }

    /// <summary>
    /// Extracts the zero-based processor numbers of all bits set in <paramref name="affinityMask"/>, up to
    /// <paramref name="affinityMaskWidth"/> bits.
    /// </summary>
    /// <param name="affinityMask">The affinity bitmask to read.</param>
    /// <param name="affinityMaskWidth">The width, in bits, of the native affinity mask.</param>
    /// <returns>The processor numbers whose bits are set, in ascending order.</returns>
    /// <remarks>
    /// Internal (rather than private) so bit extraction can be unit-tested with masks and widths that do not occur on the
    /// test machine — non-contiguous processor sets, or the 32-bit-process mask width. Materialises eagerly (rather than
    /// a yield iterator) so the caller's argument guard throws at call time instead of being deferred until enumeration.
    /// </remarks>
    internal static IEnumerable<int> GetEnabledProcessors(long affinityMask, int affinityMaskWidth)
    {
        var enabledProcessors = new List<int>();
        for (var i = 0; i < affinityMaskWidth; i++)
        {
            if ((affinityMask & (1L << i)) != 0)
            {
                enabledProcessors.Add(i);
            }
        }

        return enabledProcessors;
    }

    /// <summary>
    /// Validates that <paramref name="processorNumber"/> is representable in the affinity mask: non-negative and below
    /// <paramref name="affinityMaskWidth"/>.
    /// </summary>
    /// <param name="processorNumber">The zero-based processor number to validate.</param>
    /// <param name="affinityMaskWidth">The width, in bits, of the native affinity mask.</param>
    /// <param name="paramName">The caller's parameter name to report in the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="processorNumber"/> is not representable.</exception>
    /// <remarks>
    /// Internal (rather than private) so the mask-width capping can be unit-tested with limits that do not occur on the
    /// test machine — for example the 32-bit-process mask width. <see cref="Environment.ProcessorCount"/> is deliberately
    /// not part of the validation: it is a processor <i>count</i> (affinity-aware since .NET 6), not an index bound, so
    /// using it would reject valid processor numbers for processes constrained to non-contiguous processor sets.
    /// Processor numbers for CPUs that do not exist on the machine are rejected by the operating system when the
    /// affinity is applied.
    /// </remarks>
    internal static void ValidateProcessorNumber(int processorNumber, int affinityMaskWidth, string paramName)
    {
        if (processorNumber < 0 || processorNumber >= affinityMaskWidth)
        {
            throw new ArgumentOutOfRangeException(paramName,
                                                  processorNumber,
                                                  $"Processor number must be between 0 and {affinityMaskWidth - 1} — the width of the native processor-affinity mask ({affinityMaskWidth} bits). Processor numbers for CPUs that do not exist on this machine are rejected by the operating system when the affinity is applied.");
        }
    }
}
