using System;
using System.Text.Json;

namespace Ploch.Common.Serialization.SystemTextJson;

public class SystemTextJsonObjectConverter : JsonObjectConverter<JsonElement>
{
    protected override object? DeserializeObject(JsonElement jsonObject, Type targetType)
    {
        return jsonObject.Deserialize(targetType);
    }
}