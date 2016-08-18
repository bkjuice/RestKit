using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestKit
{
    public class Resource : IHttpResource, IDisposable
    {
        private HttpClient client;

        private ResourceEventConfiguration eventConfig;

        private Action<object, Stream> onSerialize;

        private MediaChain mediaChain;

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

        public IEventConfiguration Events
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

        public HttpClient Client
        {
            get
            {
                return this.client;
            }
        }

        public void SetMediaSerializer(Action<object, Stream> serializerAction)
        {
            this.onSerialize = serializerAction;
        }

        public void AddMediaDeserializer(Func<Stream, Type, object> deserializerFunc, string mediaType)
        {
            if (this.mediaChain == null)
            {
                this.mediaChain = new MediaChain();
            }

            this.mediaChain.AddHandler(new MediaHandler(deserializerFunc, mediaType));
        }

        public Representation Get(Uri uri)
        {
            return this.GetAsync(uri).Result;
        }

        public async Task<Representation> GetAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeGet();
            return this.HandleResult(await this.client.GetAsync(uri).ConfigureAwait(false));
        }

        public Representation Post<TRequest>(Uri uri, TRequest resource)
        {
            return this.PostAsync(uri, resource).Result;
        }

        public async Task<Representation> PostAsync<TRequest>(Uri uri, TRequest resource)
        {
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePost(content);
            return this.HandleResult(await this.client.PostAsync(uri, content).ConfigureAwait(false));
        }

        public Representation Put<TRequest>(Uri uri, TRequest resource)
        {
            return this.PutAsync(uri, resource).Result;
        }

        public async Task<Representation> PutAsync<TRequest>(Uri uri, TRequest resource)
        {
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePut(content);
            return this.HandleResult(await this.client.PutAsync(uri, content).ConfigureAwait(false));
        }

        public Representation Delete(Uri uri)
        {
            return this.DeleteAsync(uri).Result;
        }

        public async Task<Representation> DeleteAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeDelete();
            return this.HandleResult(await this.client.DeleteAsync(uri).ConfigureAwait(false));
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

        private StreamContent SerializeContent(object resource)
        {
            if (this.onSerialize == null)
            {
                throw new InvalidOperationException($"The { nameof(this.SetMediaSerializer) } callback must be set for this method.");
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
            return new Representation(reply, this.mediaChain);
        }
    }
}