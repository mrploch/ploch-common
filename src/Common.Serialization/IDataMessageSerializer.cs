using System;

namespace Ploch.Common.Serialization;

public interface IDataMessageSerializer
{
    string SerializeMessage<TMessage>(TMessage message);

    (TMessage, TData?) DeserializeAndDataMessage<TMessage, TMessageDataType, TData>(Func<TMessage, Type> dataTypeProvider,
                                                                                    Func<TMessage, TMessageDataType> messageDataTypePropertyProvider,
                                                                                    string serializedMessage);
}