using System;
using System.Collections.Generic;
using System.Reflection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Reflection;

// Caller information arguments should not be provided explicitly - this is built for .NET Standard 2.0 where caller information attributes are not available.
#pragma warning disable S3236
/// <summary>
///     Provides a fluent builder for creating a collection of assemblies.
/// </summary>
public class AssemblyListBuilder
{
    private readonly HashSet<Assembly> _assemblies = [];

    /// <summary>
    ///     Adds a single assembly to the collection.
    /// </summary>
    /// <param name="assembly">The assembly to add.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the assembly parameter is null.</exception>
    public AssemblyListBuilder AddAssembly(Assembly assembly)
    {
        assembly.NotNull(nameof(assembly));

        _assemblies.Add(assembly);

        return this;
    }

    /// <summary>
    ///     Adds multiple assemblies to the collection.
    /// </summary>
    /// <param name="assemblies">One or more collections of assemblies to add.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the assemblies parameter is null.</exception>
    public AssemblyListBuilder AddAssemblies(params IEnumerable<Assembly> assemblies)
    {
        assemblies.NotNull(nameof(assemblies));

        foreach (var assembly in assemblies)
        {
            AddAssembly(assembly);
        }

        return this;
    }

    /// <summary>
    ///     Adds the assembly containing the specified generic type to the collection.
    /// </summary>
    /// <typeparam name="T">The type whose assembly should be added.</typeparam>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    public AssemblyListBuilder AddFromType<T>() => AddAssemblies(typeof(T).GetTypeInfo().Assembly);

    /// <summary>
    ///     Adds the assembly containing the specified type to the collection.
    /// </summary>
    /// <param name="type">The type whose assembly should be added.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the type parameter is null.</exception>
    public AssemblyListBuilder AddFromType(Type type)
    {
        type.NotNull(nameof(type));

        return AddAssembly(type.GetTypeInfo().Assembly);
    }

    /// <summary>
    ///     Adds the assemblies containing the specified types to the collection.
    /// </summary>
    /// <param name="types">One or more collections of types whose assemblies should be added.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the types parameter is null.</exception>
    public AssemblyListBuilder AddFromTypes(params IEnumerable<Type> types)
    {
        types.NotNull(nameof(types));

        foreach (var type in types)
        {
            AddFromType(type);
        }

        return this;
    }

    /// <summary>
    ///     Adds the assembly containing the type of the specified object to the collection.
    /// </summary>
    /// <param name="obj">The object whose type's assembly should be added.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the obj parameter is null.</exception>
    public AssemblyListBuilder AddFromObject(object obj)
    {
        obj.NotNull(nameof(obj));

        return AddAssembly(obj.GetType().GetTypeInfo().Assembly);
    }

    /// <summary>
    ///     Adds the assemblies containing the types of the specified objects to the collection.
    /// </summary>
    /// <param name="objects">One or more collections of objects whose types' assemblies should be added.</param>
    /// <returns>The current instance of the <see cref="AssemblyListBuilder" /> to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the objects parameter is null.</exception>
    public AssemblyListBuilder AddFromObjects(params IEnumerable<object> objects)
    {
        objects.NotNull(nameof(objects));

        foreach (var obj in objects)
        {
            AddFromObject(obj);
        }

        return this;
    }

    /// <summary>
    ///     Builds and returns the collection of assemblies.
    /// </summary>
    /// <returns>An enumerable collection of the assemblies that have been added to the builder.</returns>
    public IEnumerable<Assembly> Build() => _assemblies;
}
#pragma warning restore S3236
