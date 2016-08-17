using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<TRequest> : IDisposable
    {
        IMediaConfiguration<TRequest> MediaHandler { get; }

        IEventConfiguration EventConfig { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification ="Get is a ubiquitous term in HTTP that will always be qualified by the owning instance.")]
        Response Get(Uri uri);

        Response Post(Uri uri, TRequest resource);

        Response Put(Uri uri, TRequest resource);

        Response Delete(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response> GetAsync(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response> PostAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response> PutAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response> DeleteAsync(Uri uri);

        void CancelPendingRequests();
    }
}