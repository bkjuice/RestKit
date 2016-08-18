using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<TRequest> : IHttpResource
    {
        IRequestBody<TRequest> Body { get; }

        Representation Post(Uri uri, TRequest resource);

        Representation Put(Uri uri, TRequest resource);

        Task<Representation> PostAsync(Uri uri, TRequest resource);

        Task<Representation> PutAsync(Uri uri, TRequest resource);
    }
}