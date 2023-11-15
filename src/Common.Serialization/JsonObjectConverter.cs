using System;
using Dawn;

namespace Ploch.Common.Serialization;

public abstract class JsonObjectConverter<TDataJsonObject> : IJsonObjectConverter
{
    public object? Convert(Type targetType, object jsonObject)
    {
        if (jsonObject is TDataJsonObject dataJsonObject)
        {
            return Convert(targetType, dataJsonObject);
        }

        return null;
    }

    public TTargetType? Convert<TTargetType>(object jsonObject)
    {
        return (TTargetType?)Convert(typeof(TTargetType), jsonObject);
    }

    protected abstract object? DeserializeObject(TDataJsonObject jsonObject, Type targetType);

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