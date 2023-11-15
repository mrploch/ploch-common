using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Ploch.Common.Serialization.SystemTextJson;

public class SystemTextJsonSerializer : IAsyncSerializer
{
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonSerializer() : this(JsonSerializerOptions.Default)
    { }

    public SystemTextJsonSerializer(JsonSerializerOptions options)
    {
        _options = options;
        _options.UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement;
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    public object? Deserialize(string serializedObj, Type type)
    {
        return JsonSerializer.Deserialize(serializedObj, type, _options);
    }

    public TTargetType? Deserialize<TTargetType>(string serializedObj)
    {
        return JsonSerializer.Deserialize<TTargetType>(serializedObj, _options);
    }

    public Task SerializeAsync(Stream stream, object obj, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.SerializeAsync(stream, obj, obj.GetType(), _options, cancellationToken);
    }

    public ValueTask<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync(stream, type, _options, cancellationToken);
    }

    public Task<TTargetType?> DeserializeAsync<TTargetType>(string serializedObj, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}