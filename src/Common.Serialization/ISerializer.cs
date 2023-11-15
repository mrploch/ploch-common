using System;

namespace Ploch.Common.Serialization;

public interface ISerializer
{
    string Serialize(object obj);

    object? Deserialize(string serializedObj, Type type);

    TTargetType? Deserialize<TTargetType>(string serializedObj);
}

public interface ISerializer<out TSettings> : ISerializer
{
    string Serialize(object obj, Action<TSettings> configuration);

    object Deserialize(string serializedObj, Type type, Action<TSettings> configuration);

    TTargetType? Deserialize<TTargetType>(string serializedObj, Action<TSettings> configuration);
}