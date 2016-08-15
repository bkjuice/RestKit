using System.Net.Http;

namespace RestKit
{
    public class Result<T>
    {
        private T content;

        public Result(ResourceStatus status)
        {
            this.Status = status;
        }

        public ResourceStatus Status { get; private set; }

        public bool HasContent { get; private set; }

        public T Content
        {
            get
            {
                return this.content;
            }
            internal set
            {
                this.content = value;
                this.HasContent = true;
            }
        }
    }
}
