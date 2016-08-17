using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<TRequest, TReply> : IDisposable
    {
        IMediaConfiguration<TRequest, TReply> MediaHandler { get; }

        IEventConfiguration EventConfig { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification ="Get is a ubiquitous term in HTTP that will always be qualified by the owning instance.")]
        Response<TReply> Get(Uri uri);

        Response<TReply> Post(Uri uri, TRequest resource);

        Response<TReply> Put(Uri uri, TRequest resource);

        Response<TReply> Delete(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response<TReply>> GetAsync(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response<TReply>> PostAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response<TReply>> PutAsync(Uri uri, TRequest resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Response<TReply>> DeleteAsync(Uri uri);

        void CancelPendingRequests();
    }
}