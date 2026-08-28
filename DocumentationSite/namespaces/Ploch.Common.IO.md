---
uid: Ploch.Common.IO
summary: *content
---

### Overview

`Ploch.Common.IO` gathers the input/output helpers that `System.IO` leaves to the caller. `PathUtils` handles
the path manipulations that are easy to write incorrectly — normalising a path with or without a trailing
separator, relative-path arithmetic, swapping or stripping an extension, and reducing an arbitrary string to a
safe file name. `StreamExtensions` adds `ToBytes`, which reads a stream out to a byte array instead of the
hand-rolled buffer loop that would otherwise be repeated at each call site.

The namespace also hosts `CommandLineParser` and `CommandLineInfo`, which turn a raw process command line into
an executable path plus its arguments. That is deliberately a *parsing* concern rather than a console-framework
concern: it is used for inspecting or reconstructing command lines obtained from the operating system, not for
defining an application's own option surface — for the latter, use a dedicated command-line framework.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
