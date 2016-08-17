using System.Net.Http;

namespace RestKit
{
    public class Resource<T> : Resource<T, T>, IHttpResource<T>
    {
        public Resource() : base()
        {
        }

        public Resource(HttpMessageHandler handler) : base(handler)
        {
        }
    }
}