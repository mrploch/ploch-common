---
uid: Ploch.Common.IO
summary: *content
---

### Overview

`Ploch.Common.IO` gathers the input/output helpers that `System.IO` leaves to the caller. `PathUtils` handles
the path manipulations that are easy to write incorrectly — normalising separators, relative-path arithmetic,
and combining fragments without surprises when one of them is already rooted. `StreamExtensions` covers the
routine conversions between streams, strings and byte arrays that would otherwise be repeated with a
hand-rolled buffer loop each time.

The namespace also hosts `CommandLineParser` and `CommandLineInfo`, which turn a raw process command line into
an executable path plus its arguments. That is deliberately a *parsing* concern rather than a console-framework
concern: it is used for inspecting or reconstructing command lines obtained from the operating system, not for
defining an application's own option surface — for the latter, use a dedicated command-line framework.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
