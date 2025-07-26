using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Serialization.SystemTextJson;

/// <summary>
///     The System.Text.Json implementation of <see cref="ISerializer" />.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="SystemTextJsonSerializer" /> class.
/// </remarks>
/// <param name="options">The serializer options.</param>
public class SystemTextJsonSerializer(JsonSerializerOptions options) : AsyncSerializer<JsonSerializerOptions, JsonElement>
{
    private readonly JsonSerializerOptions _options = options.NotNull(nameof(options));

    /// <summary>
    ///     Initializes a new instance of the <see cref="SystemTextJsonSerializer" /> class.
    /// </summary>
    public SystemTextJsonSerializer() : this(new JsonSerializerOptions())
    { }

    /// <inheritdoc />
    public override string Serialize(object obj) => JsonSerializer.Serialize(obj, _options);

    /// <inheritdoc />
    public override object? Deserialize(string serializedObj, Type type) => JsonSerializer.Deserialize(serializedObj, type, _options);

    /// <inheritdoc />
    public override TTargetType? Deserialize<TTargetType>(string serializedObj) where TTargetType : default =>
        JsonSerializer.Deserialize<TTargetType>(serializedObj, _options);

    /// <inheritdoc />
    public override Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default) =>
        JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), _options, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default) =>
        JsonSerializer.DeserializeAsync(stream, type, _options, cancellationToken);

    /// <inheritdoc />
    public override ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, CancellationToken cancellationToken = default)
        where TTargetType : default => JsonSerializer.DeserializeAsync<TTargetType>(stream, _options, cancellationToken);

    /// <inheritdoc />
    protected override string Serialize(object obj, JsonSerializerOptions settings) => JsonSerializer.Serialize(obj, settings);

    /// <inheritdoc />
    protected override object? Deserialize(string serializedObj, Type type, JsonSerializerOptions settings) =>
        JsonSerializer.Deserialize(serializedObj, type, settings);

    /// <inheritdoc />
    protected override TTargetType? Deserialize<TTargetType>(string serializedObj, JsonSerializerOptions settings) where TTargetType : default =>
        JsonSerializer.Deserialize<TTargetType>(serializedObj, settings);

    /// <inheritdoc />
    protected override Task SerializeAsync(Stream stream, object obj, JsonSerializerOptions settings, CancellationToken cancellationToken = default) =>
        JsonSerializer.SerializeAsync(stream, obj, settings, cancellationToken);

    /// <inheritdoc />
    protected override ValueTask<object?> DeserializeAsync(Stream stream,
                                                           Type type,
                                                           JsonSerializerOptions settings,
                                                           CancellationToken cancellationToken = default) =>
        JsonSerializer.DeserializeAsync(stream, type, settings, cancellationToken);

    /// <inheritdoc />
    protected override ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream,
                                                                             JsonSerializerOptions settings,
                                                                             CancellationToken cancellationToken = default) where TTargetType : default =>
        JsonSerializer.DeserializeAsync<TTargetType>(stream, settings, cancellationToken);

    /// <inheritdoc />
    protected override object? DeserializeObject(JsonElement jsonObject, Type targetType) => jsonObject.Deserialize(targetType);

    /// <inheritdoc />
    protected override JsonSerializerOptions GetSettings(Action<JsonSerializerOptions>? configuration)
    {
        configuration?.Invoke(_options);
        _options.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;

        return _options;
    }
}
