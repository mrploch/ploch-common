using System.Reflection;

namespace Ploch.Common.Linq;

/// <summary>
///     Represents an interface for retrieving and setting the value of a property
///     owned by an instance of an object.
/// </summary>
/// <remarks>
///     <para>
///         This interface represents a property in a specific instance of an object. Instances of this interface
///         provide an easy way to retrieve and set the value of a property.
///         It wraps the PropertyInfo and connects it with a specific owning object instance.
///     </para>
///     <para>
///         It is extensively used by the <see cref="ExpressionExtensions" /> class.
///     </para>
/// </remarks>
/// <seealso cref="ExpressionExtensions" />
/// <see cref="PropertyInfo" />
public interface IOwnedPropertyInfo
{
    /// <summary>
    ///     Gets the name of the property.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the underlying <see cref="PropertyInfo" />.
    /// </summary>
    PropertyInfo PropertyInfo { get; }

    /// <summary>
    ///     Gets the object that owns the property.
    /// </summary>
    object Owner { get; }

    /// <summary>
    ///     Retrieves a value of the property.
    /// </summary>
    /// <returns>A value of the property.</returns>
    object? GetValue();

    /// <summary>
    ///     Retrieves a value of the index property.
    /// </summary>
    /// <param name="index">The property index.</param>
    /// <returns>The property value at the specified index.</returns>
    object? GetValue(object[] index);

    /// <summary>
    ///     Sets a value of the property.
    /// </summary>
    /// <param name="value">A value to set the property to.</param>
    void SetValue(object? value);

    /// <summary>
    ///     Sets a value of the index property at the specified index.
    /// </summary>
    /// <param name="value">A value to set the property to.</param>
    /// <param name="index">The index value where to set the property value.</param>
    void SetValue(object? value, object[] index);
}

/// <summary>
///     Represents an interface for properties that belong to a specific instance of an object.
/// </summary>
/// <remarks>
///     <para>
///         This interface provides abstract methods and properties for interacting with object properties
///         at runtime in a strongly-typed manner. It encapsulates metadata about a property
///         (<see cref="PropertyInfo" />), the name of the property, and the instance that owns the property.
///     </para>
///     <para>
///         This provides an API for retrieving and modifying property values, often used in dynamic object manipulation
///         scenarios or when working with reflection.
///     </para>
/// </remarks>
/// <example>
///     <para>
///         The <c>IOwnedPropertyInfo</c> interface can be implemented to dynamically interact with object properties,
///         for example, to modify property values at runtime based on their metadata.
///     </para>
/// </example>
/// <seealso cref="PropertyInfo" />
public interface IOwnedPropertyInfo<TProperty> : IOwnedPropertyInfo
{
    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue()" />
    new TProperty? GetValue();

    /// <inheritdoc cref="IOwnedPropertyInfo.GetValue(object[])" />
    new TProperty? GetValue(object[] index);

    /// <inheritdoc cref="IOwnedPropertyInfo.SetValue(object?)" />
    void SetValue(TProperty? value);

    /// <inheritdoc cref="IOwnedPropertyInfo.SetValue(object?, object[])" />
    void SetValue(TProperty? value, object[] index);
}

/// <typeparam name="TType">The object type that owns the property.</typeparam>
/// <typeparam name="TProperty">The type of the property value.</typeparam>
/// <inheritdoc />
public interface IOwnedPropertyInfo<out TType, TProperty> : IOwnedPropertyInfo<TProperty>
{
    /// <inheritdoc cref="IOwnedPropertyInfo.Owner" />
    new TType Owner { get; }
}
