using System;

namespace Ploch.Common.Serialization;

/// <summary>
///     Common interface for serializers.
/// </summary>
public interface ISerializer
{
    /// <summary>
    ///     Serializes the specified object.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The serialized object.</returns>
    string Serialize(object obj);

    /// <summary>
    ///     Deserializes the specified serialized object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="type">The type of the object.</param>
    /// <returns>The deserialized object.</returns>
    object? Deserialize(string serializedObj, Type type);

    /// <summary>
    ///     Deserializes the specified serialized object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <typeparam name="TTargetType">The type to deserialize object to.</typeparam>
    /// <returns>The deserialized object.</returns>
    TTargetType? Deserialize<TTargetType>(string serializedObj);

    /// <summary>
    ///     Converts the embedded object to the specified type.
    /// </summary>
    /// <param name="jsonObject">The embedded object.</param>
    /// <typeparam name="TTargetType">The target type.</typeparam>
    /// <returns>The deserialized embedded object.</returns>
    TTargetType? Convert<TTargetType>(object? jsonObject);

    /// <summary>
    ///     Converts an object to the specified target type.
    /// </summary>
    /// <param name="targetType">The type to which the object should be converted.</param>
    /// <param name="jsonObject">The object to be converted. It may be null.</param>
    /// <returns>
    ///     The converted object if the conversion is successful; otherwise, null.
    /// </returns>
    object? Convert(Type targetType, object? jsonObject);
}

/// <inheritdoc />
public interface ISerializer<out TSettings> : ISerializer
{
    /// <summary>
    ///     Serializes the specified object.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="configuration">The configuration action for the serializer settings.</param>
    /// <returns>The serialized object.</returns>
    string Serialize(object obj, Action<TSettings>? configuration);

    /// <summary>
    ///     Deserializes the specified serialized object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="type">The type of the object.</param>
    /// <param name="configuration">The configuration action for the serializer settings.</param>
    /// <returns>The deserialized object.</returns>
    object? Deserialize(string serializedObj, Type type, Action<TSettings>? configuration);

    /// <summary>
    ///     Deserializes the specified serialized object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <typeparam name="TTargetType">The type to deserialize object to.</typeparam>
    /// <param name="configuration">The configuration action for the serializer settings.</param>
    /// <returns>The deserialized object.</returns>
    TTargetType? Deserialize<TTargetType>(string serializedObj, Action<TSettings>? configuration);
}
