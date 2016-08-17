using System.Net;

namespace RestKit
{
    public class ResourceStatus
    {
        public ResourceStatus()
        {
            this.ReasonPhrase = string.Empty;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string ReasonPhrase { get; set; }
    }
}
