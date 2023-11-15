using System;
using System.Diagnostics.CodeAnalysis;
using Dawn;

namespace Ploch.Common.Serialization;

public abstract class DataMessageSerializer<TDataJsonObject> : IDataMessageSerializer
{
    private readonly IJsonObjectConverter<TDataJsonObject> _jsonObjectConverter;
    private readonly ISerializer _serializer;

    protected DataMessageSerializer(ISerializer serializer, IJsonObjectConverter<TDataJsonObject> jsonObjectConverter)
    {
        _serializer = serializer;
        _jsonObjectConverter = jsonObjectConverter;
    }

    public abstract string SerializeMessage<TMessage>(TMessage message);

    [SuppressMessage("Design", "CC0031:Check for null before calling a delegate", Justification = "Parameters are checked for null.")]
    public (TMessage, TData?) DeserializeAndDataMessage<TMessage, TMessageDataType, TData>(Func<TMessage, Type> dataTypeProvider,
                                                                                           Func<TMessage, TMessageDataType> messageDataTypePropertyProvider,
                                                                                           string serializedMessage)
    {
        Guard.Argument(serializedMessage, nameof(serializedMessage)).NotNull();
        Guard.Argument(messageDataTypePropertyProvider, nameof(messageDataTypePropertyProvider)).NotNull();
        Guard.Argument(dataTypeProvider, nameof(dataTypeProvider)).NotNull();

        var message = _serializer.Deserialize<TMessage>(serializedMessage);

        var data = messageDataTypePropertyProvider(message);

        return data is not null ? (message, _jsonObjectConverter.Convert<TData>(data)) : (message, default);
    }
}