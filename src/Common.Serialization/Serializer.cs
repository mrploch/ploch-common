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
    public string Serialize(object obj, Action<TSettings> configuration)
    {
        return Serialize(obj, GetSettings(configuration));
    }
    
    /// <summary>
    /// Serializes the specified object using concrete <typeparamref name="TSettings"/>.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <returns>The serialized object.</returns>
    protected abstract string Serialize(object obj, TSettings settings);

    /// <inheritdoc />
    public object? Deserialize(string serializedObj, Type type, Action<TSettings> configuration)
    {
        return Deserialize(serializedObj, type, GetSettings(configuration));
    }

    /// <inheritdoc />
    public TTargetType? Deserialize<TTargetType>(string serializedObj, Action<TSettings> configuration)
    {
        return Deserialize<TTargetType>(serializedObj, GetSettings(configuration));
    }

    /// <inheritdoc />
    public TTargetType? Convert<TTargetType>(object jsonObject)
    {
        return (TTargetType?)Convert(typeof(TTargetType), jsonObject);
    }

    /// <summary>
    /// Deserializes the specified serialized object using concrete <typeparamref name="TSettings"/>.;
    /// </summary>
    /// <param name="serializedObj">String represeting the serialized object.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <returns>The serialized object.</returns>
    protected abstract object? Deserialize(string serializedObj, Type type, TSettings settings);

    protected abstract TTargetType? Deserialize<TTargetType>(string serializedObj, TSettings settings);

    protected abstract object? DeserializeObject(TDataJsonObject jsonObject, Type targetType);

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