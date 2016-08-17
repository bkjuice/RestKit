using System;
using System.IO;

namespace RestKit
{
    public interface IMediaConfiguration<out TRequest>
    {
        void SetSerializer(Action<TRequest, Stream> serializerAction);

        void AddDeserializer<TReply>(Func<Stream, TReply> deserializerFunc, string mediaType);
    }
}
