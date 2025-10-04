using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ploch.Common.Serialization;

/// <summary>
/// The common interface for async serializers.
/// </summary>
public interface IAsyncSerializer : ISerializer
{
    /// <summary>
    /// Asynchronously serializes the specified object.
    /// </summary>
    /// <param name="stream">The stream to write the serialized object to.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task representing an asynchronous operation.</returns>
    Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deserializes the specified serialized object.
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialization task with a deserialized object result.</returns>
    ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deserializes the specified serialized object.
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TTargetType">The type of the object to deserialize to.</typeparam>
    /// <returns>The deserialization task with a deserialized object result.</returns>
    ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, CancellationToken cancellationToken = default);
}

/// <inheritdoc cref="IAsyncSerializer"/>
public interface IAsyncSerializer<out TSettings> : IAsyncSerializer, ISerializer<TSettings>
{
    /// <summary>
    /// Asynchronously serializes the specified object.
    /// </summary>
    /// <param name="stream">The stream to write the serialized object to.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="configuration">Action to configure the serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task representing an asynchronous operation.</returns>
    Task SerializeAsync(Stream stream, object obj, Action<TSettings>? configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deserializes the specified serialized object.
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <param name="configuration">Action to configure the serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialization task with a deserialized object result.</returns>
    ValueTask<object?> DeserializeAsync(Stream stream, Type type, Action<TSettings>? configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deserializes the specified serialized object.
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="configuration">Action to configure the serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TTargetType">The type of the object to deserialize to.</typeparam>
    /// <returns>The deserialization task with a deserialized object result.</returns>
    ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, Action<TSettings>? configuration, CancellationToken cancellationToken = default);
}
