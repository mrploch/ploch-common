using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides helper methods for working with object graphs.
/// </summary>
public static class ObjectGraphHelper
{
    /// <summary>
    ///     Executes the specified action on all properties of the specified type.
    /// </summary>
    /// <remarks>
    ///     Recursively executes the specified action on all properties if the property type matches
    ///     <typeparamref name="TPropertyType" />.
    /// </remarks>
    /// <param name="root">The root object.</param>
    /// <param name="action">The action to execute on properties.</param>
    /// <typeparam name="TPropertyType">The property type.</typeparam>
    public static void ExecuteOnProperties<TPropertyType>(this object? root, Action<TPropertyType> action)
    {
        root.ExecuteOnProperties(obj =>
                                 {
                                     if (obj is TPropertyType property)
                                     {
                                         action?.Invoke(property);
                                     }
                                 });
    }

    /// <summary>
    ///     Executes the specified action on all properties.
    /// </summary>
    /// <remarks>
    ///     Recursively executes the specified action on all properties.
    /// </remarks>
    /// <param name="root">The root object.</param>
    /// <param name="action">The action to execute on properties.</param>
    public static void ExecuteOnProperties(this object? root, Action<object> action)
    {
        if (root == null)
        {
            return;
        }

        var visited = new HashSet<object>();

        ProcessProperties(root, action, visited);
    }

    private static void ProcessProperties(this object current, Action<object> action, HashSet<object> visited)
    {
        visited.Add(current);
        action?.Invoke(current);
        foreach (var property in GetProperties(current))
        {
            var value = property.GetValue(current);
            if (value == null || visited.Contains(value))
            {
                continue;
            }

            visited.Add(value);

            action?.Invoke(value);

            if (value is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    ProcessProperties(item, action, visited);
                }
            }
            else
            {
                ProcessProperties(value, action, visited);
            }
        }
    }

    private static IEnumerable<PropertyInfo> GetProperties(object obj)
    {
        var type = obj.GetType();

        return type.GetTypeInfo().GetProperties();
    }
}