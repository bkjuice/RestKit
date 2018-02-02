using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RestKit
{
    public sealed partial class Resource : IHttpResource, IPreConfiguredResource
    {
        private static readonly HttpClientPool ClientPool = new HttpClientPool();

        private HttpClient explicitInstance;

        private ResourceEventConfiguration eventConfig;

        private Action<object, Stream> onSerialize;

        private Action<HttpClient> onClientInit;

        private MediaChain mediaChain;

        public Resource() : this(default(HttpClient))
        {
        }

        public Resource(HttpMessageHandler handler) : this(new HttpClient(handler))
        {
        }

        public Resource(HttpClient client)
        {
            this.explicitInstance = client;
        }

        public IEventConfiguration Events
        {
            get
            {
                if (this.eventConfig == null)
                {
                    this.eventConfig = new ResourceEventConfiguration(this.GetClientToUse);
                }

                return this.eventConfig;
            }
        }

        public bool DoNotBuffer { get; set; }

        Resource IPreConfiguredResource.AsJson(string mediaType) => ConfigureJson(this, mediaType);

        Resource IPreConfiguredResource.AsJson() => ConfigureJson(this, DefaultMedia.ApplicationJson);

        Resource IPreConfiguredResource.AsXml(string mediaType) => ConfigureXml(this, mediaType);

        Resource IPreConfiguredResource.AsXml() => ConfigureXml(this, DefaultMedia.TextXml);

        Resource IPreConfiguredResource.AsText(string mediaType) => ConfigureText(this, mediaType);

        Resource IPreConfiguredResource.AsText() => ConfigureText(this, DefaultMedia.TextPlain);

        Resource IPreConfiguredResource.AsMedia(string mediaType, Action<object, Stream> serializer, Func<Stream, Type, object> deserializer)
        {
            return ConfigureContent(this, serializer, deserializer, mediaType);
        }

        Resource IPreConfiguredResource.AsRaw() => this;

        public void SetMediaSerializer(Action<object, Stream> serializerAction) => this.onSerialize = serializerAction;

        public void AddMediaDeserializer(Func<Stream, Type, object> deserializerFunc, string mediaType)
        {
            if (this.mediaChain == null)
            {
                this.mediaChain = new MediaChain();
            }

            this.mediaChain.AddHandler(new MediaHandler(deserializerFunc, mediaType));
        }

        public Representation Get(Uri uri) => this.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Representation> GetAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeAction(uri);
            this.eventConfig?.InvokeOnBeforeGet(uri);
            return this.HandleResult(await this.GetClientToUse(uri).GetAsync(uri).ConfigureAwait(false));
        }

        public Representation Post<TRequest>(Uri uri, TRequest resource) => this.PostAsync(uri, resource).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Representation> PostAsync<TRequest>(Uri uri, TRequest resource)
        {
            this.eventConfig?.InvokeOnBeforeAction(uri);
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePost(content, uri);
            return this.HandleResult(await this.GetClientToUse(uri).PostAsync(uri, content).ConfigureAwait(false));
        }

        public Representation Put<TRequest>(Uri uri, TRequest resource) => this.PutAsync(uri, resource).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Representation> PutAsync<TRequest>(Uri uri, TRequest resource)
        {
            this.eventConfig?.InvokeOnBeforeAction(uri);
            var content = this.SerializeContent(resource);
            this.eventConfig?.InvokeOnBeforePut(content, uri);
            return this.HandleResult(await this.GetClientToUse(uri).PutAsync(uri, content).ConfigureAwait(false));
        }

        public Representation Delete(Uri uri) => this.DeleteAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<Representation> DeleteAsync(Uri uri)
        {
            this.eventConfig?.InvokeOnBeforeAction(uri);
            this.eventConfig?.InvokeOnBeforeDelete(uri);
            return this.HandleResult(await this.GetClientToUse(uri).DeleteAsync(uri).ConfigureAwait(false));
        }

        public void CancelPendingRequests()
        {
            // There could be a collection of pending requests managed by this instance such that they can all be canceled...
            // However, this could cancel all pending requests unintentionally for a pooled client.
            // Throw invalid operation exception to ensure usage is as expected:
            if (this.explicitInstance == null)
            {
                throw new InvalidOperationException("To be able to cancel pending requests, you must initialize the resource with an existing HTTP Client instance that you provided.");
            }

            this.explicitInstance.CancelPendingRequests();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private HttpClient GetClientToUse(Uri uri) => this.explicitInstance ?? ClientPool.GetClient(uri, this.onClientInit);

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
            var result = new Representation(reply, !DoNotBuffer, this.mediaChain);
            this.eventConfig?.InvokeReplyActions(result);
            return result;
        }
    }
}