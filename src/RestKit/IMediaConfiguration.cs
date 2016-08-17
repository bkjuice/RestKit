using System;
using System.IO;
using System.Net.Http;

namespace RestKit
{
    public interface IMediaConfiguration<out TRequest, TReply>
    {
        void SetSerializer(Action<TRequest, Stream> serializerAction);

        void AddDeserializer(Func<Stream, TReply> deserializerFunc, string mediaType);
    }
}
