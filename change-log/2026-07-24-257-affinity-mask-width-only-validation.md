# ProcessExtensions: validate processor affinity against the native mask width only (#257)

## Summary

`SetSingleProcessorAffinity`, `SetEnabledProcessors` and `GetEnabledProcessors` in
`Ploch.Common.Diagnostics.ProcessExtensions` no longer use `Environment.ProcessorCount` as an upper bound for
processor numbers. Since .NET 6, `Environment.ProcessorCount` is the number of processors *available to the
process* (it already reflects the process's own affinity and CPU limits) — it is a count, not an index bound, so
using it rejected valid processor numbers and under-reported enabled processors for processes constrained to
non-contiguous processor sets (for example CPUs 8–15, where `ProcessorCount` is 8).

## Changes

- Processor numbers are now validated only against the native affinity-mask width (`IntPtr.Size * 8` — 32 in a
  32-bit process, 64 in a 64-bit process). Processor numbers for CPUs that do not exist on the machine are
  rejected by the operating system when the affinity is applied (typically as `Win32Exception`).
- `GetEnabledProcessors` reports every bit set in the native mask (bounded by the mask width alone), so
  non-contiguous affinity sets are reported correctly.
- XML documentation updated on all three methods to describe the new contract.

## Behaviour change (breaking)

Calls with a processor number that is within the native mask width but greater than or equal to
`Environment.ProcessorCount` previously threw `ArgumentOutOfRangeException` up front. They are now passed to the
operating system: valid targets (processors the machine has, e.g. under a constrained affinity) succeed, and
nonexistent processors surface as an OS-level error (typically `Win32Exception`) when the affinity is applied.
Code that relied on the eager `ArgumentOutOfRangeException` for such inputs must be updated.
