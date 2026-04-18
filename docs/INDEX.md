# Ploch.Common Documentation Hub

Welcome to the comprehensive documentation for Ploch.Common — a suite of .NET utility libraries that simplify everyday
development tasks.

## Documentation Guide

| Resource                                    | Best For                                                |
|---------------------------------------------|---------------------------------------------------------|
| [Getting Started Guide](GETTING_STARTED.md) | New users — installation, first steps, common use cases |
| [Quick Reference](QUICK_REFERENCE.md)       | Experienced users — method signatures, cheat sheet      |
| [API Reference](API_REFERENCE.md)           | Complete method reference organised by namespace        |
| [Main README](https://github.com/mrploch/ploch-common#readme)                 | Full feature overview with in-depth examples            |

## Library Documentation

Per-library documentation with overviews, key types, usage examples, and configuration guidance.

### Core Foundation

| Library                      | Description                                                                          | Docs                                                               |
|------------------------------|--------------------------------------------------------------------------------------|--------------------------------------------------------------------|
| Ploch.Common                 | Extension methods for strings, collections, types, paths, guard clauses, randomisers | [common.md](libraries/common.md)                                   |
| Ploch.Common.Net9            | .NET 9+ specific extensions (AppDomainTypesLoader)                                   | [common-net9.md](libraries/common-net9.md)                         |
| Ploch.Common.DataAnnotations | Custom validation attributes (RequiredNotDefaultDate)                                | [common-data-annotations.md](libraries/common-data-annotations.md) |
| Ploch.Common.ObjectBuilder   | WMI/COM object-to-POCO conversion pipeline                                           | [common-object-builder.md](libraries/common-object-builder.md)     |
| Ploch.Common.Ardalis.Result  | Result-to-HTTP status code mapping extensions                                        | [common-ardalis-result.md](libraries/common-ardalis-result.md)     |

### Serialization

| Library                                                                 | Description                                                       | Docs                                                                                                 |
|-------------------------------------------------------------------------|-------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|
| Ploch.Common.Serialization                                              | Abstract serialisation interfaces (ISerializer, IAsyncSerializer) | [common-serialization.md](libraries/common-serialization.md)                                         |
| Ploch.Common.Serialization.SystemTextJson                               | System.Text.Json implementation (sync + async)                    | [common-serialization-system-text-json.md](libraries/common-serialization-system-text-json.md)       |
| Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection | DI registration for System.Text.Json serialiser                   | [common-serialization-system-text-json-di.md](libraries/common-serialization-system-text-json-di.md) |
| Ploch.Common.Serialization.NewtonsoftJson                               | Newtonsoft.Json implementation (sync only)                        | [common-serialization-newtonsoft-json.md](libraries/common-serialization-newtonsoft-json.md)         |
| Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection | DI registration for Newtonsoft.Json serialiser                    | [common-serialization-newtonsoft-json-di.md](libraries/common-serialization-newtonsoft-json-di.md)   |

### Dependency Injection

| Library                                  | Description                                      | Docs                                                                                       |
|------------------------------------------|--------------------------------------------------|--------------------------------------------------------------------------------------------|
| Ploch.Common.DependencyInjection         | ServicesBundle pattern — modular DI registration | [common-dependency-injection.md](libraries/common-dependency-injection.md)                 |
| Ploch.Common.DependencyInjection.Autofac | Autofac container integration                    | [common-dependency-injection-autofac.md](libraries/common-dependency-injection-autofac.md) |
| Ploch.Common.DependencyInjection.Hosting | IHostBuilder integration for ServicesBundle      | [common-dependency-injection-hosting.md](libraries/common-dependency-injection-hosting.md) |
| Ploch.Common.Extensions.Configuration    | Configuration section binding extensions         | [common-extensions-configuration.md](libraries/common-extensions-configuration.md)         |
| Ploch.Common.Dependencies                | Meta-package for shared dependencies             | [common-dependencies.md](libraries/common-dependencies.md)                                 |

### Web, API, and Applications

| Library                                  | Description                                                     | Docs                                                             |
|------------------------------------------|-----------------------------------------------------------------|------------------------------------------------------------------|
| Ploch.Common.WebApi                      | CRUD endpoint builders, AutoMapper integration, FastEndpoints   | [common-webapi.md](libraries/common-webapi.md)                   |
| Ploch.Common.Web                         | Swagger/OpenAPI configuration helpers                           | [common-web.md](libraries/common-web.md)                         |
| Ploch.Common.WebUI                       | Razor/MVC page utilities (AppPage, SelectListHelper)            | [common-webui.md](libraries/common-webui.md)                     |
| Ploch.Common.AppServices                 | Application service abstractions (IUserInfoProvider)            | [common-appservices.md](libraries/common-appservices.md)         |
| Ploch.Common.AppServices.Web             | HTTP context user info provider                                 | [common-appservices-web.md](libraries/common-appservices-web.md) |
| Ploch.Common.Apps                        | Action handler framework (IActionHandler, ActionHandlerManager) | [common-apps.md](libraries/common-apps.md)                       |
| Ploch.Common.UseCases                    | Use case abstractions (placeholder)                             | [common-usecases.md](libraries/common-usecases.md)               |
| Ploch.Common.NSwag                       | NSwag operation name generator                                  | [common-nswag.md](libraries/common-nswag.md)                     |
| Ploch.Common.Maui                        | MAUI MVVM bases, view discovery, font management                | [common-maui.md](libraries/common-maui.md)                       |
| Ploch.Common.Windows                     | Windows-specific utilities (placeholder)                        | [common-windows.md](libraries/common-windows.md)                 |
| Ploch.Common.Windows.DependencyInjection | Windows DI registration (placeholder)                           | [common-windows-di.md](libraries/common-windows-di.md)           |

### Testing Support

| Library                                              | Description                                                                  | Docs                                                                                         |
|------------------------------------------------------|------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------|
| Ploch.TestingSupport                                 | Base testing utilities, test data attributes, FluentVerifier                 | [testing-support.md](libraries/testing-support.md)                                           |
| Ploch.TestingSupport.XUnit3                          | xUnit v3 integration with custom data attributes and OS-conditional skipping | [testing-support-xunit3.md](libraries/testing-support-xunit3.md)                             |
| Ploch.TestingSupport.XUnit3.AutoMoq                  | AutoFixture + Moq integration for xUnit v3                                   | [testing-support-xunit3-automoq.md](libraries/testing-support-xunit3-automoq.md)             |
| Ploch.TestingSupport.FluentAssertions                | Custom FluentAssertions extensions                                           | [testing-support-fluent-assertions.md](libraries/testing-support-fluent-assertions.md)       |
| Ploch.TestingSupport.FluentAssertions.IOAbstractions | IO Abstractions FluentAssertions extensions                                  | [testing-support-fluent-assertions-io.md](libraries/testing-support-fluent-assertions-io.md) |
| Ploch.TestingSupport.MockConsoleApp                  | Stub executable for process-launch integration tests                         | [testing-support-mock-console-app.md](libraries/testing-support-mock-console-app.md)         |
| Ploch.TestingSupport.Dependencies.MetaPackages       | Meta-package .nuspec repository                                              | [testing-support-meta-packages.md](libraries/testing-support-meta-packages.md)               |
| Ploch.TestingSupport.XUnit3.Dependencies             | xUnit v3 testing dependency meta-package                                     | [testing-support-xunit3-dependencies.md](libraries/testing-support-xunit3-dependencies.md)   |
| Ploch.TestingSupport.XUnit2.Dependencies             | xUnit v2 testing dependency meta-package (legacy)                            | [testing-support-xunit2-dependencies.md](libraries/testing-support-xunit2-dependencies.md)   |

### Build Configuration

| Resource                      | Description                                                        | Docs                                             |
|-------------------------------|--------------------------------------------------------------------|--------------------------------------------------|
| Build Configuration (MSBuild) | Directory.Build.props, Central Package Management, NBGV versioning | [common-msbuild.md](libraries/common-msbuild.md) |

## Quick Navigation by Feature

| Feature              | Getting Started             | Quick Reference                                         | API Reference                                          | Library Docs                                          |
|----------------------|-----------------------------|---------------------------------------------------------|--------------------------------------------------------|-------------------------------------------------------|
| String operations    | [Guide](GETTING_STARTED.md) | [Cheat sheet](QUICK_REFERENCE.md#string-extensions)     | [API](API_REFERENCE.md#string-extensions)              | [Ploch.Common](libraries/common.md)                   |
| Collections          | [Guide](GETTING_STARTED.md) | [Cheat sheet](QUICK_REFERENCE.md#collection-extensions) | [API](API_REFERENCE.md#collection-extensions)          | [Ploch.Common](libraries/common.md)                   |
| Guard clauses        | [Guide](GETTING_STARTED.md) | [Cheat sheet](QUICK_REFERENCE.md#guard-clauses)         | [API](API_REFERENCE.md#guard-parameter-validation)     | [Ploch.Common](libraries/common.md)                   |
| Serialisation        | [Guide](GETTING_STARTED.md) | —                                                       | [API](API_REFERENCE.md#plochcommonserialization)       | [Serialization](libraries/common-serialization.md)    |
| Dependency injection | [Guide](GETTING_STARTED.md) | —                                                       | [API](API_REFERENCE.md#plochcommondependencyinjection) | [DI](libraries/common-dependency-injection.md)        |
| Testing              | [Guide](GETTING_STARTED.md) | —                                                       | —                                                      | [TestingSupport](libraries/testing-support-xunit3.md) |

## External Resources

- [GitHub Repository](https://github.com/mrploch/ploch-common)
- [NuGet Packages](https://www.nuget.org/packages?q=Ploch.Common)
- [Issue Tracker](https://github.com/mrploch/ploch-common/issues)

---

[Back to Main README](https://github.com/mrploch/ploch-common#readme)
