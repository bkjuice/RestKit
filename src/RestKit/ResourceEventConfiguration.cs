using System;
using System.Linq;
using System.Net.Http;

namespace RestKit
{
    public class ResourceEventConfiguration : IEventConfiguration
    {
        private HttpClient client;

        private Action<HttpClient> onBeforeGet;

        private Action<HttpClient, HttpContent> onBeforePost;

        private Action<HttpClient, HttpContent> onBeforePut;

        private Action<HttpClient> onBeforeDelete;

        private Action<HttpResponseMessage> onReply;

        private Action<HttpResponseMessage> onInformationStatus;

        private Action<HttpResponseMessage> onSuccessStatus;

        private Action<HttpResponseMessage> onRedirectStatus;

        private Action<HttpResponseMessage> onClientErrorStatus;

        private Action<HttpResponseMessage> onServerErrorStatus;

        private Action<HttpResponseMessage> onContentMismatch;

        public ResourceEventConfiguration(HttpClient client)
        {
            this.client = client;
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

        public void OnInformationStatus(Action<HttpResponseMessage> action)
        {
            this.onInformationStatus += action;
        }

        public void OnSuccessStatus(Action<HttpResponseMessage> action)
        {
            this.onSuccessStatus += action;
        }

        public void OnRedirectStatus(Action<HttpResponseMessage> action)
        {
            this.onRedirectStatus += action;
        }

        public void OnClientErrorStatus(Action<HttpResponseMessage> action)
        {
            this.onClientErrorStatus += action;
        }

        public void OnServerErrorStatus(Action<HttpResponseMessage> action)
        {
            this.onServerErrorStatus += action;
        }

        public void OnContentMismatch(Action<HttpResponseMessage> action)
        {
            this.onContentMismatch += action;
        }

        public void InvokeOnBeforeGet()
        {
            this.onBeforeGet?.Invoke(this.client);
        }

        public void InvokeOnBeforePost(HttpContent content)
        {
            this.onBeforePost?.Invoke(this.client, content);
        }

        public void InvokeOnBeforePut(HttpContent content)
        {
            this.onBeforePut?.Invoke(this.client, content);
        }

        public void InvokeOnBeforeDelete()
        {
            this.onBeforeDelete?.Invoke(this.client);
        }

        public void InvokeReplyActions(HttpResponseMessage reply)
        {
            var statusCode = (int)reply.StatusCode;
            this.onReply?.Invoke(reply);
            var accepts = reply.RequestMessage?.Headers?.Accept;
            var content = reply.Content?.Headers?.ContentType;

            var match = accepts?.FirstOrDefault(h => h.MediaType.Equals(content?.MediaType, StringComparison.OrdinalIgnoreCase)) != null;
            if (!match)
            {
                this.onContentMismatch?.Invoke(reply);
            }

            var code = (int)reply.StatusCode;
            if (code < 200)
            {
                this.onInformationStatus?.Invoke(reply);
                return;
            }

            if (code < 300)
            {
                this.onSuccessStatus?.Invoke(reply);
                return;
            }

            if (code < 400)
            {
                this.onRedirectStatus?.Invoke(reply);
                return;
            }

            if (code < 500)
            {
                this.onClientErrorStatus?.Invoke(reply);
                return;
            }

            this.onServerErrorStatus?.Invoke(reply);
        }
    }
}