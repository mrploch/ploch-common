using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides methods for calculating hash codes for objects based on their internal state and values.
///     The <see cref="ObjectHashCodeBuilder" /> class is designed to compute hash codes by utilizing reflection
///     to inspect readable, non-indexed properties of objects in a deterministic manner. It supports cyclic references
///     by tracking visited objects to avoid infinite recursion during computation, and it can process sequences
///     (e.g., <see cref="System.Collections.IEnumerable" />) by incorporating their elements.
/// </summary>
public static class ObjectHashCodeBuilder
{
    /// <summary>
    ///     Computes a hash code for the specified object, considering its internal state and values.
    ///     The method inspects readable, non-indexed properties and processes sequences, while supporting cyclic references
    ///     to avoid infinite recursion. If the object is <c>null</c>, this method returns <c>0</c>.
    /// </summary>
    /// <param name="obj">The object for which the hash code should be calculated. Can be <c>null</c>.</param>
    /// <returns>An integer representing the computed hash code for the given object.</returns>
    public static int GetHashCode(object? obj)
    {
        if (obj is null)
        {
            return 0;
        }

        // Track visited references to avoid infinite recursion on cyclic graphs.
        var visited = new HashSet<object>(new ReferenceEqualityComparer());

        return ComputeObjectHash(obj!, visited);
    }

    private static int ComputeObjectHash(object value, HashSet<object> visited) => ComputeValueHash(value, visited);

    private static int ComputeValueHash(object? value, HashSet<object> visited)
    {
        unchecked
        {
            if (value is null)
            {
                return 0;
            }

            var type = value.GetType();

            if (type.IsSimpleType())
            {
                return value.GetHashCode();
            }

            // Prevent cycles for reference types
            if (!type.IsValueType)
            {
                if (!visited.Add(value))
                {
                    // Already processed this reference; inject a marker and stop.
                    return unchecked((int)0x9E3779B9); // golden ratio constant, forced to int
                }
            }

            // Handle sequences (but not string which is IEnumerable<char>)
            if (value is IEnumerable enumerable && value is not string)
            {
                var h = type.GetHashCode(); // seed with a sequence type
                foreach (var item in enumerable)
                {
                    var itemHash = ComputeValueHash(item, visited);
                    h = Combine(h, itemHash);
                }

                return h;
            }

            // Fallback: reflect over readable, non-indexed instance properties in name order for determinism
            var props = type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                        .OrderBy(p => p.Name, StringComparer.Ordinal)
                        .ToArray();

            if (props.Length == 0)
            {
                // If no properties, base the hash on the type itself to keep it stable.
                return type.GetHashCode();
            }

            var hash = type.GetHashCode(); // include type to distinguish different types with same property values

            foreach (var p in props)
            {
                object? propValue;
                try
                {
                    propValue = p.GetValue(value);
                }
                catch
                {
                    // In case property getter throws, incorporate property name to maintain determinism
                    hash = Combine(hash, p.Name.GetHashCode());

                    continue;
                }

                hash = Combine(hash, p.Name.GetHashCode());
                var propHash = ComputeValueHash(propValue, visited);
                hash = Combine(hash, propHash);
            }

            return hash;
        }
    }

    private static int Combine(int h1, int h2) => unchecked((h1 * 31) ^ h2);

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
