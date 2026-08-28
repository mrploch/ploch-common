---
uid: Ploch.Common.Extensions.Configuration
summary: *content
---

### Overview

`Ploch.Common.Extensions.Configuration` removes the string literal from options binding. The conventional
`services.Configure<MyOptions>(configuration.GetSection("MyOptions"))` repeats the section name at every
registration site and silently binds nothing when that name drifts away from the class it describes. Marking
the options class with `ConfigurationSectionAttribute` moves the name onto the type itself, and the extension
methods on `ConfigurationOptionsExtensions` then read it back, so the name is declared once rather than
repeated at every registration site. The attribute takes the section name as an explicit string, so renaming
the options class does not update it — pass `nameof(MyOptions)` if you want the two to stay in step.

The namespace is intentionally small: it is a thin, dependency-light convenience over
`Microsoft.Extensions.Configuration` and `Microsoft.Extensions.Options`, and it does not replace either. Reach
for it whenever an application binds more than a handful of options classes and you would rather not maintain
a parallel set of section-name constants.

See the [Ploch.Common.Extensions.Configuration library
guide](../../docs/libraries/common-extensions-configuration.md) for installation instructions and worked
examples.
