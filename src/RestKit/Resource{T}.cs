using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestKit
{
    public class Resource<TRequest> : IHttpResource<TRequest>, IMediaConfiguration<TRequest>, IDisposable
    {
        private HttpClient client;

        private ResourceEventConfiguration eventConfig;

        private Action<TRequest, Stream> onSerialize;

        private MediaHandler handlerHead;

        private MediaHandler handlerTail;

        public Resource() : this(new HttpClient())
        {
        }

        public Resource(HttpMessageHandler handler) : this(new HttpClient(handler))
        {
        }

        private Resource(HttpClient client)
        {
            this.client = client;
        }

        public IEventConfiguration EventConfig
        {
            get
            {
                if (this.eventConfig == null)
                {
                    this.eventConfig = new ResourceEventConfiguration(this.client);
                }

                return this.eventConfig;
            }
        }

        public IMediaConfiguration<TRequest> MediaHandler
        {
            get
            {
                return this;
            }
        }

        public HttpClient Client
        {
            get
            {
                return this.client;
            }
        }

        public void SetSerializer(Action<TRequest, Stream> serializerAction)
        {
            this.onSerialize = serializerAction;
        }

        public void AddDeserializer<TReply>(Func<Stream, TReply> deserializerFunc, string mediaType)
        {
            var handler = new MediaHandler(s => deserializerFunc(s), mediaType);
            if (handlerHead == null)
            {
                this.handlerHead = handler;
                this.handlerTail = handler;
            }
            else
            {
                this.handlerTail.Next = handler;
                this.handlerTail = handler;
            }
        }

        public Representation Get(Uri uri)
        {
            return this.GetAsync(uri).Result;
        }

        public async Task<Representation> GetAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeGet();

            var accepts = this.client.DefaultRequestHeaders.Accept;
            using (var reply = await this.client.GetAsync(uri).ConfigureAwait(false))
            {
                return this.HandleResult(reply);
            }
        }

        public Representation Post(Uri uri, TRequest resource)
        {
            return this.PostAsync(uri, resource).Result;
        }

        public async Task<Representation> PostAsync(Uri uri, TRequest resource)
        {
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePost(content);
            using (var reply = await this.client.PostAsync(uri, content).ConfigureAwait(false))
            {
                return this.HandleResult(reply);
            }
        }

        public Representation Put(Uri uri, TRequest resource)
        {
            return this.PutAsync(uri, resource).Result;
        }

        public async Task<Representation> PutAsync(Uri uri, TRequest resource)
        {
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePut(content);
            using (var reply = await this.client.PutAsync(uri, content).ConfigureAwait(false))
            {
                return this.HandleResult(reply);
            }
        }

        public Representation Delete(Uri uri)
        {
            return this.DeleteAsync(uri).Result;
        }

        public async Task<Representation> DeleteAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeDelete();
            using (var reply = await this.client.DeleteAsync(uri).ConfigureAwait(false))
            {
                return this.HandleResult(reply);
            }
        }

        public void CancelPendingRequests()
        {
            this.client.CancelPendingRequests();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.client?.Dispose();
            }
        }

        private StreamContent SerializeContent(TRequest resource)
        {
            if (this.onSerialize == null)
            {
                throw new InvalidOperationException($"The { nameof(this.SetSerializer) } callback must be set for this method.");
            }

            var s = new MemoryStream(4096);
            this.onSerialize(resource, s);
            s.Flush();
            s.Position = 0;

            return new StreamContent(s);
        }

        private Representation HandleResult(HttpResponseMessage reply)
        {
            // TODO: handle 100 and 300 codes, and allow for retry via RetryPolicy, with no touch defaults
            var statusCode = reply.StatusCode;
            this.eventConfig?.InvokeReplyActions(reply);
            return new Representation(reply, this.handlerHead);
        }
    }
}