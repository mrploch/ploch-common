using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Linq;

namespace Ploch.Common.Reflection;

/// <summary>
///     Common reflection tasks convenience object extensions.
/// </summary>
public static class PropertyHelpers
{
    /// <summary>
    ///     Represents the default name for an indexer property in .NET, i.e., "Item".
    ///     This constant is commonly used in reflection-based operations to reference
    ///     indexer properties implemented in classes.
    /// </summary>
    /// <remarks>
    ///     In .NET, indexers are special properties that allow instances of a class
    ///     or struct to be indexed like arrays. The default name for indexers is "Item",
    ///     but the actual usage can vary based on the language or implementation.
    ///     This constant provides a strongly typed reference to that default name
    ///     to avoid hardcoded strings in reflection utilities.
    /// </remarks>
    /// <example>
    ///     <code>
    /// // Example usage of the `IndexerPropertyName` constant in reflection.
    /// var propertyValue = someObject.GetPropertyValue(PropertyHelpers.IndexerPropertyName, new object[] { 0 });
    /// Console.WriteLine(propertyValue);
    /// </code>
    /// </example>
    public const string IndexerPropertyName = "Item";

    /// <summary>
    ///     Gets the <see langword="public" /> properties of a specific type.
    /// </summary>
    /// <typeparam name="TPropertyType">
    ///     The type of the properties to return.
    /// </typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="includeAssignableOrInheritedTypes">
    ///     Include subtypes of <typeparamref name="TPropertyType" /> and types that are assignable to in
    ///     results.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="obj" /> is <see langword="null" />.
    /// </exception>
    /// <returns>
    ///     List of <see langword="public" /> properties of a specific type.(
    ///     <see cref="PropertyInfo" /> s).
    /// </returns>
    public static IEnumerable<PropertyInfo> GetProperties<TPropertyType>(this object obj, bool includeAssignableOrInheritedTypes = true)
    {
        obj.NotNull(nameof(obj));

        return obj.GetProperties(includeAssignableOrInheritedTypes, typeof(TPropertyType));
    }

    /// <summary>
    ///     Retrieves public properties of a specified object based on a set of property types,
    ///     optionally including assignable or inherited types.
    /// </summary>
    /// <param name="obj">
    ///     The object whose properties are to be retrieved. Must not be <see langword="null" />.
    /// </param>
    /// <param name="includeAssignableOrInheritedTypes">
    ///     A value indicating whether subtypes of the specified property types or types assignable
    ///     to them should be included in the results.
    /// </param>
    /// <param name="propertyTypes">
    ///     The types of the properties to retrieve. This can include one or more target property types.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="obj" /> is <see langword="null" />.
    /// </exception>
    /// <returns>
    ///     A collection of <see cref="PropertyInfo" /> objects corresponding to the properties
    ///     of the specified type(s) that belong to the given object.
    /// </returns>
    public static IEnumerable<PropertyInfo> GetProperties(this object obj, bool includeAssignableOrInheritedTypes, params Type[] propertyTypes)
    {
        obj.NotNull(nameof(obj));

        var type = obj.GetType();
        var propertyTypesSet = new HashSet<Type>(propertyTypes);

        return type.GetTypeInfo()
                   .GetProperties()
                   .Where(pi =>
                          {
                              if (!includeAssignableOrInheritedTypes)
                              {
                                  return propertyTypesSet.Contains(pi.PropertyType);
                              }

                              return propertyTypesSet.Any(pt => pt.GetTypeInfo().IsAssignableFrom(pi.PropertyType));
                          });
    }

    /// <summary>
    ///     Gets the property information.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="throwIfNotFound">
    ///     if set to <c>true</c> throws
    ///     <see cref="PropertyNotFoundException" /> if property is not found.
    /// </param>
    /// <exception cref="PropertyNotFoundException">
    ///     if <paramref name="throwIfNotFound" /> is <c>true</c> and property
    ///     is not found.
    /// </exception>
    /// <returns>
    ///     Property information.
    /// </returns>
    public static PropertyInfo? GetPropertyInfo(this Type type, string propertyName, bool throwIfNotFound)
    {
        type.NotNull(nameof(type));
        propertyName.NotNullOrEmpty(nameof(propertyName));

        var propertyInfo = type.GetTypeInfo().GetProperty(propertyName);

        if (propertyInfo == null && throwIfNotFound)
        {
            throw new PropertyNotFoundException(propertyName);
        }

        return propertyInfo;
    }

    /// <summary>
    ///     Gets all property names and their values from the specified object.
    /// </summary>
    /// <remarks>
    ///     This method uses reflection to retrieve all public properties of the object's type
    ///     and returns their names paired with their current values. Properties that cannot
    ///     be read or throw exceptions during value retrieval may cause this method to fail.
    /// </remarks>
    /// <example>
    ///     <code>
    /// var person = new { Name = "John", Age = 30, City = "New York" };
    /// var propertyValues = person.GetPropertyValues();
    ///
    /// foreach (var (name, value) in propertyValues)
    /// {
    ///     Console.WriteLine($"{name}: {value}");
    /// }
    /// // Output:
    /// // Name: John
    /// // Age: 30
    /// // City: New York
    /// </code>
    /// </example>
    /// <param name="obj">The object from which to retrieve property values.</param>
    /// <returns>
    ///     An enumerable collection of tuples containing property names and their corresponding values.
    ///     Each tuple contains the property name as a string and the property value as an object (which may be null).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="obj" /> is <see langword="null" />.
    /// </exception>
    public static IEnumerable<(string Name, object? Value)> GetPropertyValues(this object obj)
    {
        obj.NotNull(nameof(obj));

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type - this is a false/positive in this case.
        return obj.GetType().GetProperties().Where(pi => pi.GetIndexParameters().Length == 0).Select(pi => (pi.Name, pi.GetValue(obj)));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }

    /// <summary>
    ///     Gets the property value.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="index">Optional index value.</param>
    /// <exception cref="AmbiguousMatchException">
    ///     More than one property is found with the specified name. See
    ///     Remarks.
    /// </exception>
    /// <exception cref="PropertyNotFoundException">
    ///     If <paramref name="propertyName" /> property is not found.
    /// </exception>
    /// <returns>
    ///     Property value.
    /// </returns>
    public static object? GetPropertyValue<T>(this T obj, string propertyName, object?[]? index = null)
    {
        obj.NotNull(nameof(obj));
        propertyName.NotNullOrEmpty(nameof(propertyName));

        var propertyInfo = obj!.GetType().GetPropertyInfo(propertyName, true);
        PropertyAccessValidators.ValidatePropertyInfoForGetValue(propertyInfo, propertyName, index);

        return propertyInfo.RequiredNotNull(nameof(propertyInfo)).GetValue(obj, index);
    }

    /// <summary>
    ///     Gets the property value.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <typeparam name="TValue">The type of the retrieved property value.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="index">Optional index value.</param>
    /// <exception cref="AmbiguousMatchException">
    ///     More than one property is found with the specified name. See
    ///     Remarks.
    /// </exception>
    /// <exception cref="PropertyNotFoundException">
    ///     If <paramref name="propertyName" /> property is not found.
    /// </exception>
    /// <returns>
    ///     Property value.
    /// </returns>
    public static TValue? GetPropertyValue<T, TValue>(this T obj, string propertyName, object?[]? index = null) =>
        (TValue?)GetPropertyValue(obj, propertyName, index);

    /// <summary>
    ///     Gets the property value.
    /// </summary>
    /// <param name="obj">The object to get the property from.</param>
    /// <param name="propertyInfo">The property info of the property to get the value from.</param>
    /// <param name="index">Optional index value.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>The property value.</returns>
    public static object? GetPropertyValue<T>(this T? obj, PropertyInfo propertyInfo, object?[]? index = null)
    {
        propertyInfo.NotNull(nameof(propertyInfo));

        PropertyAccessValidators.ValidatePropertyInfoForGetValue(propertyInfo, propertyInfo.Name, index);

        return index != null ? propertyInfo.GetValue(obj, index) : propertyInfo.GetValue(obj);
    }

    /// <summary>
    ///     Gets the value of a property from an object using a lambda expression to specify the property.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to retrieve.</typeparam>
    /// <typeparam name="T">The type of the object containing the property.</typeparam>
    /// <param name="obj">The object from which to retrieve the property value.</param>
    /// <param name="propertySelector">A lambda expression specifying the property to retrieve (e.g., x => x.PropertyName).</param>
    /// <returns>
    ///     The value of the specified property, or null if the property doesn't exist or its value is null.
    ///     The return type matches the type of the property specified in the expression.
    /// </returns>
    public static TProperty? GetPropertyValue<TProperty, T>(this T obj, Expression<Func<T, TProperty>> propertySelector)
    {
        obj.NotNull(nameof(obj));
        propertySelector.NotNull(nameof(propertySelector));

        var property = obj.GetProperty(propertySelector);

        return property.GetValue();
    }

    /// <summary>
    ///     Retrieves the value of a static property of a given type.
    /// </summary>
    /// <param name="type">The type that contains the static property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <exception cref="InvalidOperationException">If property was not found in the provided type.</exception>
    /// <returns> The value of the static property.</returns>
    public static object? GetStaticPropertyValue(this Type type, string propertyName)
    {
        if (!type.TryGetStaticPropertyValue(propertyName, out var value))
        {
            throw new InvalidOperationException($"Static property {propertyName} was not found in {type}");
        }

        return value;
    }

    /// <summary>
    ///     Retrieves the value of a static property from the specified type.
    /// </summary>
    /// <typeparam name="TValue">The type of the property value.</typeparam>
    /// <param name="type">The type that contains the static property.</param>
    /// <param name="propertyName">The name of the static property.</param>
    /// <returns>
    ///     The value of the static property, if it exists and is of type TValue;
    ///     otherwise, null is returned if the property does not exist,
    ///     or an <see cref="InvalidOperationException" /> is thrown if the property exists but is not of type TValue.
    /// </returns>
    public static TValue? GetStaticPropertyValue<TValue>(this Type type, string propertyName)
    {
        type.NotNull(nameof(type));
        propertyName.NotNullOrEmpty(nameof(propertyName));

        var valueObj = GetStaticPropertyValue(type, propertyName);

        return valueObj switch
               { null => default,
                 TValue value => value,
                 _ => throw new InvalidOperationException($"Static property {propertyName} in {type} is not of {typeof(TValue)} type") };
    }

    /// <summary>
    ///     Determines whether the specified property name has property.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <exception cref="AmbiguousMatchException">
    ///     More than one property is found with the specified name. See
    ///     Remarks.
    /// </exception>
    /// <returns>
    ///     <c>true</c> if the specified property name has property; otherwise,
    ///     <c>false</c> .
    /// </returns>
    public static bool HasProperty(this object obj, string propertyName) => obj.NotNull(nameof(obj)).GetType().GetPropertyInfo(propertyName, false) != null;

    /// <summary>
    ///     Sets the property.
    /// </summary>
    /// <typeparam name="T">The type of property.</typeparam>
    /// <param name="obj">The object type.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="AmbiguousMatchException">
    ///     More than one property is found with the specified name. See
    ///     Remarks.
    /// </exception>
    /// <exception cref="TargetException">
    ///     In the .NET for Windows Store apps or the Portable Class Library,
    ///     <see langword="catch" /> <see cref="Exception" /> instead. The type
    ///     of <paramref name="obj" /> does not match the target type, or a
    ///     property is an instance property but <paramref name="obj" /> is
    ///     null.
    /// </exception>
    /// <exception cref="MethodAccessException">
    ///     In the .NET for Windows Store apps or the Portable Class Library,
    ///     <see langword="catch" /> the base class exception,
    ///     <see cref="MemberAccessException" /> , instead. There was an illegal
    ///     attempt to access a <see langword="private" /> or
    ///     <see langword="protected" /> method inside a class.
    /// </exception>
    /// <exception cref="TargetInvocationException">
    ///     An error occurred while setting the property value. The
    ///     <see cref="System.Exception.InnerException" /> property indicates
    ///     the reason for the error.
    /// </exception>
    /// <exception cref="PropertyNotFoundException">
    ///     If <paramref name="propertyName" /> property is not found.
    /// </exception>
    public static void SetPropertyValue<T>(this T obj, string propertyName, object? value)
    {
        obj.NotNull(nameof(obj));
        propertyName.NotNullOrEmpty(nameof(propertyName));

        var propertyInfo = typeof(T).GetPropertyInfo(propertyName, true);

        if (propertyInfo == null)
        {
            throw new PropertyNotFoundException(propertyName);
        }

        propertyInfo.SetValue(obj, value);
    }

    /// <summary>
    ///     Tries to get the value of a static property from a type.
    /// </summary>
    /// <param name="type">The type from which to get the static property value.</param>
    /// <param name="propertyName">The name of the static property to get.</param>
    /// <param name="value">The value of the static property, if it exists.</param>
    /// <returns>True if the static property exists and its value can be retrieved, false otherwise.</returns>
    public static bool TryGetStaticPropertyValue(this Type type, string propertyName, out object? value)
    {
        type.NotNull(nameof(type));
        propertyName.NotNullOrEmpty(nameof(propertyName));

        var propertyInfo = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);

        if (propertyInfo == null)
        {
            value = null;

            return false;
        }

        value = propertyInfo.GetValue(null);

        return true;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="PropertyInfo" /> represents a static property.
    /// </summary>
    /// <param name="propertyInfo">
    ///     The <see cref="PropertyInfo" /> instance to evaluate.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the property is static; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="propertyInfo" /> is <see langword="null" />.
    /// </exception>
    public static bool IsStatic(this PropertyInfo propertyInfo) => propertyInfo.NotNull(nameof(propertyInfo)).GetAccessors(true)[0].IsStatic;
}
