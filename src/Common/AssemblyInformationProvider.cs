using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Provides methods to retrieve information about the assembly of an object or a type.
/// </summary>
public static class AssemblyInformationProvider
{
    /// <summary>
    ///     Retrieves the assembly information for the given object.
    /// </summary>
    /// <param name="obj">The object to get the assembly information for.</param>
    /// <returns>The <see cref="AssemblyInformation" /> representing the assembly information.</returns>
    public static AssemblyInformation GetAssemblyInformation(this object obj)
    {
        obj.NotNull(nameof(obj));

        return new(obj.GetType().Assembly);
    }

    /// <summary>
    ///     Gets the assembly information for the specified <paramref name="type" />.
    /// </summary>
    /// <param name="type">The type to retrieve the assembly information for.</param>
    /// <returns>An instance of <see cref="AssemblyInformation" /> containing the assembly information.</returns>
    public static AssemblyInformation GetAssemblyInformation(this Type type)
    {
        type.NotNull(nameof(type));

        return new(type.Assembly);
    }
}
