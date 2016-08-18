using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource : IDisposable
    {
        IEventConfiguration Events { get; }

        HttpClient Client { get; }

        Representation Get(Uri uri);

        Representation Post<TRequest>(Uri uri, TRequest resource);

        Representation Put<TRequest>(Uri uri, TRequest resource);

        Representation Delete(Uri uri);

        Task<Representation> GetAsync(Uri uri);

        Task<Representation> PostAsync<TRequest>(Uri uri, TRequest resource);

        Task<Representation> PutAsync<TRequest>(Uri uri, TRequest resource);

        Task<Representation> DeleteAsync(Uri uri);

        void CancelPendingRequests();

        void SetMediaSerializer(Action<object, Stream> serializerAction);

        void AddMediaDeserializer(Func<Stream, Type, object> deserializerFunc, string mediaType);
    }
}