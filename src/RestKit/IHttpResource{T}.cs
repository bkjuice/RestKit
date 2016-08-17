using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<TRequest> : IDisposable
    {
        IMediaConfiguration<TRequest> MediaHandler { get; }

        IEventConfiguration EventConfig { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "Get is a ubiquitous term in HTTP that will always be qualified by the owning instance.")]
        Representation Get(Uri uri);

        Representation Post(Uri uri, TRequest resource);

        Representation Put(Uri uri, TRequest resource);

        Representation Delete(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Representation> GetAsync(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Representation> PostAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Representation> PutAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Representation> DeleteAsync(Uri uri);

        void CancelPendingRequests();
    }
}