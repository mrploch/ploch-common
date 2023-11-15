using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ploch.Common.Serialization;

public interface IAsyncSerializer : ISerializer
{
    Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default);

    ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

    Task<TTargetType?> DeserializeAsync<TTargetType>(string serializedObj, CancellationToken cancellationToken = default);
}

public interface IAsyncSerializer<TSettings> : IAsyncSerializer, ISerializer<TSettings>
{
    Task SerializeAsync(Stream stream, object obj, Action<TSettings> configuration, CancellationToken cancellationToken = default);

    Task<object> DeserializeAsync(string serializedObj, Type type, Action<TSettings> configuration, CancellationToken cancellationToken = default);

    Task<TTargetType> DeserializeAsync<TTargetType>(string serializedObj, Action<TSettings> configuration, CancellationToken cancellationToken = default);
}