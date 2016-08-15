using System;
using System.IO;
using System.Net.Http;

namespace RestKit
{
    public interface IResourceConfiguration<T>
    {
        HttpClient Client { get; }

        void OnBeforeGet(Action<HttpClient> action);

        void OnBeforePost(Action<HttpClient, HttpContent> action);

        void OnBeforePut(Action<HttpClient, HttpContent> action);

        void OnBeforeDelete(Action<HttpClient> action);

        void OnReply(Action<HttpResponseMessage> action);

        void SetSerializer(Action<T, Stream> serializerAction);

        void SetDeserializer(Func<Stream, T> deserializerFunc);

        void SetCanDeserialize(Func<HttpResponseMessage, bool> handler);
    }
}
