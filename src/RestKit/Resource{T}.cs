using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestKit
{
    public sealed class Resource<T> : IHttpResource<T>, IResourceConfiguration<T>
    {
        private HttpClient client;

        private Action<HttpClient> onBeforeGet;

        private Action<HttpClient, HttpContent> onBeforePost;

        private Action<HttpClient, HttpContent> onBeforePut;

        private Action<HttpClient> onBeforeDelete;

        private Action<T, Stream> onSerialize;

        private Func<Stream, T> onDeserialize;

        private Func<HttpResponseMessage, bool> canDeserialize;

        private Action<HttpResponseMessage> onReply;

        public Resource() : this(new HttpClient())
        {
        }

        public Resource(HttpMessageHandler handler) : this(new HttpClient(handler))
        {
        }

        private Resource(HttpClient client)
        {
            this.client = client;
            this.canDeserialize = CanDeserialize;
        }

        public IResourceConfiguration<T> Config
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

        public void OnBeforeGet(Action<HttpClient> action)
        {
            this.onBeforeGet += action;
        }

        public void OnBeforePost(Action<HttpClient, HttpContent> action)
        {
            this.onBeforePost += action;
        }

        public void OnBeforePut(Action<HttpClient, HttpContent> action)
        {
            this.onBeforePut += action;
        }

        public void OnBeforeDelete(Action<HttpClient> action)
        {
            this.onBeforeDelete += action;
        }

        public void OnReply(Action<HttpResponseMessage> action)
        {
            this.onReply += action;
        }

        public void SetCanDeserialize(Func<HttpResponseMessage, bool> handler)
        {
            if (handler != null)
            {
                this.canDeserialize = handler;
            }
        }

        public void SetSerializer(Action<T, Stream> serializerAction)
        {
            this.onSerialize = serializerAction;
        }

        public void SetDeserializer(Func<Stream, T> deserializerFunc)
        {
            this.onDeserialize = deserializerFunc;
        }

        public void Dispose()
        {
            this.client?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Result<T> Get(Uri uri)
        {
            return this.GetAsync(uri).Result;
        }

        public async Task<Result<T>> GetAsync(Uri uri)
        {
            this.onBeforeGet?.Invoke(this.client);
            using (var reply = await this.client.GetAsync(uri).ConfigureAwait(false))
            {
                return await this.HandleResult(reply);
            }
        }

        public Result<T> Post(Uri uri, T resource)
        {
            return this.PostAsync(uri, resource).Result;
        }

        public async Task<Result<T>> PostAsync(Uri uri, T resource)
        {
            var content = this.GetResourceContent(resource);
            this.onBeforePost?.Invoke(this.client, content);
            using (var reply = await this.client.PostAsync(uri, content).ConfigureAwait(false))
            {
                return await this.HandleResult(reply);
            }
        }

        public Result<T> Put(Uri uri, T resource)
        {
            return this.PutAsync(uri, resource).Result;
        }

        public async Task<Result<T>> PutAsync(Uri uri, T resource)
        {
            var content = this.GetResourceContent(resource);
            this.onBeforePut?.Invoke(this.client, content);
            using (var reply = await this.client.PutAsync(uri, content).ConfigureAwait(false))
            {
                return await this.HandleResult(reply);
            }
        }

        public Result<T> Delete(Uri uri)
        {
            return this.DeleteAsync(uri).Result;
        }

        public async Task<Result<T>> DeleteAsync(Uri uri)
        {
            this.onBeforeDelete?.Invoke(this.client);
            using (var reply = await this.client.DeleteAsync(uri).ConfigureAwait(false))
            {
                return await this.HandleResult(reply);
            }
        }

        public void CancelPendingRequests()
        {
            this.client.CancelPendingRequests();
        }

        private static bool CanDeserialize(HttpResponseMessage reply)
        {
            var code = (int)reply.StatusCode;
            var length = reply.Content?.Headers?.ContentLength;
            return code != 204 && (length > 0 || reply.Headers?.TransferEncodingChunked == true);
        }

        private StreamContent GetResourceContent(T resource)
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

        private T Deserialize(Stream data)
        {
            if (this.onDeserialize == null)
            {
                throw new InvalidOperationException($"The '{ nameof(this.SetDeserializer) }' callback must be set for this method.");
            }

            return this.onDeserialize(data);
        }

        private async Task<Result<T>> HandleResult(HttpResponseMessage reply)
        {
            this.onReply?.Invoke(reply);
            var status = new ResourceStatus { StatusCode = reply.StatusCode, StatusReason = reply.ReasonPhrase };
            var result = new Result<T>(status);

            // TODO: handle 100 and 300 codes.
            if (this.canDeserialize(reply))
            {
                using (var data = await reply.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    if (data.CanRead)
                    {
                        result.Content = this.Deserialize(data);
                    }
                }
            }

            return result;
        }
    }
}