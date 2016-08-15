using System;
using System.Threading.Tasks;

namespace RestKit
{
    public interface IHttpResource<T> : IDisposable
    {
        IResourceConfiguration<T> Config { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification ="Get is a ubiquitous term in HTTP that will always be qualified by the owning instance.")]
        Result<T> Get(Uri uri);

        Result<T> Post(Uri uri, T resource);

        Result<T> Put(Uri uri, T resource);

        Result<T> Delete(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Result<T>> GetAsync(Uri uri);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Result<T>> PostAsync(Uri uri, T resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Result<T>> PutAsync(Uri uri, T resource);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by async language feature.")]
        Task<Result<T>> DeleteAsync(Uri uri);

        void CancelPendingRequests();
    }
}