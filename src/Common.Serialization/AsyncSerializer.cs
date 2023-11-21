using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ploch.Common.Serialization;

/// <summary>
/// Base class for async serializers.
/// </summary>
/// <typeparam name="TSettings">The type of the serializer settings.</typeparam>
/// <typeparam name="TDataJsonObject">The type that the serializer uses for unknown types.</typeparam>
public abstract class AsyncSerializer<TSettings, TDataJsonObject> : Serializer<TSettings, TDataJsonObject>, IAsyncSerializer<TSettings>
{
    /// <inheritdoc />
    public abstract Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public async Task SerializeAsync(Stream stream, object obj, Action<TSettings>? configuration, CancellationToken cancellationToken = default)
    {
        await SerializeAsync(stream, obj, GetSettings(configuration), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<object?> DeserializeAsync(Stream stream, Type type, Action<TSettings> configuration, CancellationToken cancellationToken = default)
    {
        return DeserializeAsync(stream, type, GetSettings(configuration), cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, Action<TSettings> configuration, CancellationToken cancellationToken = default)
    {
        return DeserializeAsync<TTargetType>(stream, GetSettings(configuration), cancellationToken);
    }
    
    /// <summary>
    /// The method that serializes the object to the stream using concrete <typeparamref name="TSettings"/>.
    /// </summary>
    /// <param name="stream">The stream to serialize the object to.</param>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task representing he serialization.</returns>
    protected abstract Task SerializeAsync(Stream stream, object obj, TSettings settings, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Asynchronously deserializes the specified serialized object using concrete &lt;typeparamref name="TSettings"/&gt;
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task representing he serialization.</returns>
    protected abstract ValueTask<object?> DeserializeAsync(Stream stream, Type type, TSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deserializes the specified serialized object.
    /// </summary>
    /// <param name="stream">The stream for the serialized object.</param>
    /// <param name="settings">The serializer settings.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TTargetType">The type of the object to deserialize to.</typeparam>
    /// <returns>The deserialization task with a deserialized object result.</returns>
    protected abstract ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, TSettings settings, CancellationToken cancellationToken = default);
}