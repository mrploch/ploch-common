using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploch.Common.Serialization;

namespace Ploch.Common.Serialiation.NewtonsoftJson;

public class NewtonsoftJsonObjectSerializer : Serializer<JsonSerializerSettings, JObject>
{
    private readonly JsonSerializerSettings _settings;

    public NewtonsoftJsonObjectSerializer() : this(null)
    { }

    public NewtonsoftJsonObjectSerializer(JsonSerializerSettings? settings)
    {
        _settings = settings ?? new JsonSerializerSettings();
    }

    public override string Serialize(object obj)
    {
        return JsonConvert.SerializeObject(obj, _settings);
    }

    public override object? Deserialize(string serializedObj, Type type)
    {
        return JsonConvert.DeserializeObject(serializedObj, type, _settings);
    }

    public override TTargetType? Deserialize<TTargetType>(string serializedObj) where TTargetType : default
    {
        return JsonConvert.DeserializeObject<TTargetType>(serializedObj, _settings);
    }

    protected override string Serialize(object obj, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(obj, settings);
    }

    protected override object? Deserialize(string serializedObj, Type type, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject(serializedObj, type, settings);
    }

    protected override TTargetType? Deserialize<TTargetType>(string serializedObj, JsonSerializerSettings settings) where TTargetType : default
    {
        return JsonConvert.DeserializeObject<TTargetType>(serializedObj, settings);
    }

    protected override object? DeserializeObject(JObject jsonObject, Type targetType)
    {
        return jsonObject.ToObject(targetType);
    }

    protected override JsonSerializerSettings GetSettings(Action<JsonSerializerSettings>? configuration)
    {
        configuration?.Invoke(_settings);

        return _settings;
    }
}