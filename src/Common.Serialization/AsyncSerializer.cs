using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dawn;

namespace Ploch.Common.Serialization;

public abstract class AsyncSerializer<TSettings, TDataJsonObject> : IAsyncSerializer<TSettings>
{
    public abstract string Serialize(object obj);

    public abstract object? Deserialize(string serializedObj, Type type);

    public abstract TTargetType? Deserialize<TTargetType>(string serializedObj);

    public abstract Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default);

    public abstract ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

    public abstract Task<TTargetType?> DeserializeAsync<TTargetType>(string serializedObj, CancellationToken cancellationToken = default);

    public abstract string Serialize(object obj, Action<TSettings> configuration);

    public abstract object Deserialize(string serializedObj, Type type, Action<TSettings> configuration);

    public abstract TTargetType? Deserialize<TTargetType>(string serializedObj, Action<TSettings> configuration);

    public abstract Task SerializeAsync(Stream stream, object obj, Action<TSettings> configuration, CancellationToken cancellationToken = default);

    public abstract Task<object> DeserializeAsync(string serializedObj, Type type, Action<TSettings> configuration, CancellationToken cancellationToken = default);

    public abstract Task<TTargetType> DeserializeAsync<TTargetType>(string serializedObj, Action<TSettings> configuration, CancellationToken cancellationToken = default);

    public TTargetType? Convert<TTargetType>(object jsonObject)
    {
        return (TTargetType?)Convert(typeof(TTargetType), jsonObject);
    }

    protected abstract object? DeserializeObject(TDataJsonObject jsonObject, Type targetType);

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