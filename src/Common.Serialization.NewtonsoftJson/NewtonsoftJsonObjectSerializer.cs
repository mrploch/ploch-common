using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialization;

namespace Ploch.Common.Serialiation.NewtonsoftJson;

/// <summary>
/// Newtonsoft.Json based implementation of <see cref="ISerializer"/>.
/// </summary>
public class NewtonsoftJsonObjectSerializer : Serializer<JsonSerializerSettings, JObject>
{
    private readonly JsonSerializerSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonObjectSerializer"/> class.
    /// </summary>
    public NewtonsoftJsonObjectSerializer() : this(null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewtonsoftJsonObjectSerializer"/> class.
    /// </summary>
    /// <param name="settings">The serializer settings.</param>
    public NewtonsoftJsonObjectSerializer(JsonSerializerSettings? settings)
    {
        _settings = settings ?? new JsonSerializerSettings();
    }

    /// <inheritdoc />
    public override string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, _settings);
    }

    /// <inheritdoc />
    public override object? Deserialize(string serializedObj, Type type)
    {
        return JsonConvert.DeserializeObject(serializedObj, type, _settings);
    }

    /// <inheritdoc />
    public override TTargetType? Deserialize<TTargetType>(string serializedObj) where TTargetType : default
    {
        return JsonConvert.DeserializeObject<TTargetType>(serializedObj, _settings);
    }

    /// <inheritdoc />
    protected override string Serialize(object obj, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(obj, settings);
    }

    /// <inheritdoc />
    protected override object? Deserialize(string serializedObj, Type type, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject(serializedObj, type, settings);
    }

    /// <inheritdoc />
    protected override TTargetType? Deserialize<TTargetType>(string serializedObj, JsonSerializerSettings settings) where TTargetType : default
    {
        return JsonConvert.DeserializeObject<TTargetType>(serializedObj, settings);
    }

    /// <inheritdoc />
    protected override object? DeserializeObject(JObject jsonObject, Type targetType)
    {
        return jsonObject.ToObject(targetType);
    }

    /// <inheritdoc />
    protected override JsonSerializerSettings GetSettings(Action<JsonSerializerSettings>? configuration)
    {
        configuration?.Invoke(_settings);

        return _settings;
    }
}