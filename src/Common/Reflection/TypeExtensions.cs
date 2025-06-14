using System;
using System.Collections;
using System.Reflection;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;

namespace Ploch.Common.Reflection;

/// <summary>
///     <see cref="System.Type" /> extension methods.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Checks if the specified type is a concrete implementation of the provided generic base type.
    /// </summary>
    /// <typeparam name="TBaseType">The generic base type against which to validate the implementation.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="type" /> is a concrete implementation of <typeparamref name="TBaseType" />,
    ///     otherwise <c>false</c>.
    /// </returns>
    public static bool IsConcreteImplementation<TBaseType>(this Type type) => IsConcreteImplementation(type, typeof(TBaseType));

    /// <summary>
    ///     Checks if the specified type is a concrete implementation of the provided interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="baseType">The base type against which to validate the implementation.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="type" /> is a concrete implementation of the <paramref name="baseType" />,
    ///     otherwise <c>false</c>.
    /// </returns>
    public static bool IsConcreteImplementation(this Type type, Type baseType)
    {
        baseType.NotNull(nameof(baseType));
        type.NotNull(nameof(type));

        return type.IsImplementing(baseType, true);
    }

    /// <summary>
    ///     Checks if the type provided is an <see cref="IEnumerable" />.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the provided type is <see cref="IEnumerable" />, otherwise false.</returns>
    public static bool IsEnumerable(this Type type)
    {
        type.NotNull(nameof(type));

        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    /// <summary>
    ///     Checks if the type provided is implementing the specified interface.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="baseType">The type of interface.</param>
    /// <param name="concreteOnly">
    ///     Specify <c>true</c> to return <c>true</c> only if the <paramref name="type" /> is not an
    ///     interface or an abstract class.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="type" /> is implementing <paramref name="baseType" />, false
    ///     otherwise.
    /// </returns>
    public static bool IsImplementing(this Type type, Type baseType, bool concreteOnly = false)
    {
        baseType.NotNull(nameof(baseType));
        type.NotNull(nameof(type));
        if (concreteOnly && (type.IsAbstract || !type.IsClass))
        {
            return false;
        }

        if (type == baseType)
        {
            return false;
        }

        return baseType.IsAssignableFrom(type) ||
               type.GetInterfaces().Exists(i => i == baseType || (i.IsGenericType && i.GetGenericTypeDefinition() == baseType));
    }

    /// <summary>
    ///     Determines whether the specified type is a nullable type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    ///     <c>true</c> if the specified <paramref name="type" /> is a nullable type; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="type" /> is <c>null</c>.
    /// </exception>
    public static bool IsNullable(this Type type)
    {
        type.NotNull(nameof(type));

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    ///     Determines whether the specified type is a "simple" type.
    ///     A "simple" type is a primitive type, an enumeration, <see cref="string" />,
    ///     <see cref="decimal" />, or a nullable version of a simple type.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="type" /> is considered simple; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSimpleType(this Type type)
    {
        type.NotNull(nameof(type));

        var typeInfo = type.GetTypeInfo();

        if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // nullable type, check if the nested type is simple.
            return IsSimpleType(typeInfo.GetGenericArguments()[0]);
        }

        return typeInfo.IsPrimitive || typeInfo.IsValueType || typeInfo.IsEnum || type == typeof(string) || type == typeof(decimal);
    }
}
