using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Dawn;

namespace Ploch.Common.Serialization.SystemTextJson;

public class SystemTextJsonSerializer : AsyncSerializer<JsonSerializerOptions, JsonElement>
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonSerializer() : this(new JsonSerializerOptions())
    { }

    public SystemTextJsonSerializer(JsonSerializerOptions options)
    {
        _options = Guard.Argument(options, nameof(options)).NotNull();
        _options.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;
    }

    public override string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public override object? Deserialize(string serializedObj, Type type)
    {
        return JsonSerializer.Deserialize(serializedObj, type, _options);
    }

    public override TTargetType? Deserialize<TTargetType>(string serializedObj) where TTargetType : default
    {
        return JsonSerializer.Deserialize<TTargetType>(serializedObj, _options);
    }

    public override Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), _options, cancellationToken);
    }

    public override ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync(stream, type, _options, cancellationToken);
    }

    public override ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, CancellationToken cancellationToken = default) where TTargetType : default
    {
        return default;
    }

    protected override string Serialize(object obj, JsonSerializerOptions settings)
    {
        return JsonSerializer.Serialize(obj, settings);
    }

    protected override object? Deserialize(string serializedObj, Type type, JsonSerializerOptions settings)
    {
        return JsonSerializer.Deserialize(serializedObj, type, settings);
    }

    protected override TTargetType? Deserialize<TTargetType>(string serializedObj, JsonSerializerOptions settings) where TTargetType : default
    {
        return JsonSerializer.Deserialize<TTargetType>(serializedObj, settings);
    }

    protected override Task SerializeAsync(Stream stream, object obj, JsonSerializerOptions settings, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.SerializeAsync(stream, obj, settings, cancellationToken);
    }

    protected override ValueTask<object?> DeserializeAsync(Stream stream, Type type, JsonSerializerOptions settings, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync(stream, type, settings, cancellationToken);
    }

    protected override ValueTask<TTargetType?> DeserializeAsync<TTargetType>(Stream stream, JsonSerializerOptions settings, CancellationToken cancellationToken = default)
        where TTargetType : default
    {
        return JsonSerializer.DeserializeAsync<TTargetType>(stream, settings, cancellationToken);
    }

    protected override object? DeserializeObject(JsonElement jsonObject, Type targetType)
    {
        return jsonObject.Deserialize(targetType);
    }

    protected override JsonSerializerOptions GetSettings(Action<JsonSerializerOptions>? configuration)
    {
        configuration?.Invoke(_options);
        _options.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;

        return _options;
    }
}