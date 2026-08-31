---
uid: Ploch.Common
summary: *content
---

### Overview

`Ploch.Common` is the root namespace of the suite and the home of the general-purpose extension methods that
almost every other package builds on. It exists so that the small, repetitive helpers every codebase ends up
writing — null-safe string checks, enum parsing, date and byte-size formatting, environment probing, shallow
object cloning, `IsIn`-style membership tests — live in one tested, documented place instead of being
re-implemented per project. Most of its members are static extension classes, so the behaviour becomes
available simply by importing the namespace.

Reach for this namespace when you want an expression to read more clearly at the call site. The more
specialised concerns live in child namespaces — <xref:Ploch.Common.Collections> for sequence and dictionary
work, <xref:Ploch.Common.ArgumentChecking> for guard clauses, <xref:Ploch.Common.Reflection> for type and
property inspection, <xref:Ploch.Common.IO> for paths and streams — and each is documented separately.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
