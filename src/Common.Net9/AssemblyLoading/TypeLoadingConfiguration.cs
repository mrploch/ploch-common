using Microsoft.Extensions.FileSystemGlobbing;

namespace Ploch.Common.AssemblyLoading;

/// <summary>
///     Represents the configuration for type loading in an application domain. This class provides
///     options to configure how assemblies and types are matched during the type-loading process
///     and specifies the base types to use when identifying compatible types.
/// </summary>
public record TypeLoadingConfiguration(Action<Matcher>? AssemblyNameGlobConfiguration = null,
                                       Action<Matcher>? TypeNameGlobConfiguration = null,
                                       params IEnumerable<Type>? BaseTypes);
