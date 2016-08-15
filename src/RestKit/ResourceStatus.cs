using System.Net;

namespace RestKit
{
    public class ResourceStatus
    {
        public ResourceStatus()
        {
            this.StatusReason = string.Empty;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusReason { get; set; }
    }
}
