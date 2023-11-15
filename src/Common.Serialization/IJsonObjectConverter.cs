using System;

namespace Ploch.Common.Serialization;

public interface IJsonObjectConverter
{
    object? Convert(Type targetType, object jsonObject);

    TTargetType? Convert<TTargetType>(object jsonObject);
}

public interface IJsonObjectConverter<in TJsonObject> : IJsonObjectConverter
{
    object? Convert(Type targetType, TJsonObject jsonObject);

    TTargetType? Convert<TTargetType>(TJsonObject jsonObject);
}