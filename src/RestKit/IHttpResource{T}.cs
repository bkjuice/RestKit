using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<TRequest> : IHttpResource
    {
        IMediaConfiguration<TRequest> MediaHandler { get; }

        Representation Post(Uri uri, TRequest resource);

        Representation Put(Uri uri, TRequest resource);

        Task<Representation> PostAsync(Uri uri, TRequest resource);

        Task<Representation> PutAsync(Uri uri, TRequest resource);
    }
}