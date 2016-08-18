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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "Get is a ubiquitous term in HTTP that will always be qualified by the owning instance.")]
        Representation Get(Uri uri);

        Representation Delete(Uri uri);

        Task<Representation> GetAsync(Uri uri);

        Task<Representation> DeleteAsync(Uri uri);

        void CancelPendingRequests();

        void AddMediaDeserializer(IMediaHandler handler);

        void AddMediaDeserializer<TReply>(Func<Stream, TReply> deserializerFunc, string mediaType);
    }
}