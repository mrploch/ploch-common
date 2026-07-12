# fix(common): ProcessExtensions affinity mask respects the native mask width

**Issue:** [#249](https://github.com/mrploch/ploch-common/issues/249)

## Summary

`SetSingleProcessorAffinity`, `SetEnabledProcessors` and `GetEnabledProcessors` in
`Ploch.Common.Diagnostics.ProcessExtensions` validated processor numbers only against
`Environment.ProcessorCount`, ignoring the width of the native `Process.ProcessorAffinity`
bitmask (`IntPtr.Size * 8`). On machines with more than 64 logical processors the
`1L << n` shift wrapped (`n & 63`), setting the wrong bit; in a 32-bit process, bits
32–63 were silently truncated and `GetEnabledProcessors` mis-reported.

## Changes

- Processor numbers are now validated against the lesser of `Environment.ProcessorCount`
  and the native affinity-mask width. Unaddressable processor numbers throw
  `ArgumentOutOfRangeException` with a message stating both limits.
- `GetEnabledProcessors` bounds its read loop the same way, so it never reports
  processors that cannot be represented in the mask.

## Behaviour change (not breaking for supported configurations)

Previously, on hardware/process combinations where a processor number passed the
`ProcessorCount` check but exceeded the mask width, the methods silently applied a
wrong affinity mask. These calls now throw `ArgumentOutOfRangeException`. On machines
with ≤64 logical processors running 64-bit processes (the overwhelmingly common case)
behaviour is unchanged.
