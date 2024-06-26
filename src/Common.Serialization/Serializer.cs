using System;
using Dawn;

namespace Ploch.Common.Serialization;

/// <inheritdoc />
public abstract class Serializer<TSettings, TDataJsonObject> : ISerializer<TSettings>
{
    /// <inheritdoc />
    public abstract string Serialize(object obj);

    /// <inheritdoc />
    public abstract object? Deserialize(string serializedObj, Type type);

    /// <inheritdoc />
    public abstract TTargetType? Deserialize<TTargetType>(string serializedObj);

    /// <inheritdoc />
    public string Serialize(object obj, Action<TSettings>? configuration)
    {
        return Serialize(obj, GetSettings(configuration));
    }

    /// <inheritdoc />
    public object? Deserialize(string serializedObj, Type type, Action<TSettings>? configuration)
    {
        return Deserialize(serializedObj, type, GetSettings(configuration));
    }

    /// <inheritdoc />
    public TTargetType? Deserialize<TTargetType>(string serializedObj, Action<TSettings>? configuration)
    {
        return Deserialize<TTargetType>(serializedObj, GetSettings(configuration));
    }

    /// <inheritdoc />
    public TTargetType? Convert<TTargetType>(object jsonObject)
    {
        return (TTargetType?)Convert(typeof(TTargetType), jsonObject);
    }

    /// <summary>
    ///     Serializes the specified object using concrete <typeparamref name="TSettings" />.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <returns>The serialized object.</returns>
    protected abstract string Serialize(object obj, TSettings settings);

    /// <summary>
    ///     Deserializes the specified serialized object using concrete <typeparamref name="TSettings"/>.
    /// </summary>
    /// <param name="serializedObj">String representing the serialized object.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <returns>The deserialized object.</returns>
    protected abstract object? Deserialize(string serializedObj, Type type, TSettings settings);

    /// <summary>
    ///     Deserializes the specified serialized object using concrete <typeparamref name="TSettings"/>.
    /// </summary>
    /// <param name="serializedObj">String representing the serialized object.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <typeparam name="TTargetType">The type of the object to deserialize to.</typeparam>
    /// <returns>The deserialized object.</returns>
    protected abstract TTargetType? Deserialize<TTargetType>(string serializedObj, TSettings settings);

    /// <summary>
    ///     Deserializes the embedded JSON object.
    /// </summary>
    /// <param name="jsonObject">The JSON object.</param>
    /// <param name="targetType">The type to deserialize to.</param>
    /// <returns>The deserialized object.</returns>
    protected abstract object? DeserializeObject(TDataJsonObject jsonObject, Type targetType);

    /// <summary>
    ///     Retrieves and configures the serializer settings.
    /// </summary>
    /// <param name="configuration">The serializer settings configuration.</param>
    /// <returns>The serializer settings.</returns>
    protected abstract TSettings GetSettings(Action<TSettings>? configuration);

    private object? Convert(Type targetType, object jsonObject)
    {
        if (jsonObject is TDataJsonObject dataJsonObject)
        {
            return Convert(targetType, dataJsonObject);
        }

        return null;
    }

    private object? Convert(Type targetType, TDataJsonObject jsonObject)
    {
        Guard.Argument(targetType, nameof(targetType)).NotNull();

        if (jsonObject is null)
        {
            return null;
        }

        return DeserializeObject(jsonObject, targetType);
    }
}