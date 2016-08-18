using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;

namespace RestKit
{
    public class ResourceEventConfiguration : IEventConfiguration
    {
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

        private HttpClient client;

        public ResourceEventConfiguration(HttpClient client)
        {
            Contract.Requires<ArgumentNullException>(client != null);
            this.client = client;
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

        public void InvokeReplyActions(Representation representation)
        {
            var statusCode = (int)representation.StatusCode;

            var message = representation.Message;
            this.onReply?.Invoke(message);
            
            if (representation.IsUnexpectedMediaType)
            {
                this.onContentMismatch?.Invoke(message);
            }

            var code = (int)representation.StatusCode;
            if (code < 200)
            {
                this.onInformationStatus?.Invoke(message);
                return;
            }

            if (code < 300)
            {
                this.onSuccessStatus?.Invoke(message);
                return;
            }

            if (code < 400)
            {
                this.onRedirectStatus?.Invoke(message);
                return;
            }

            if (code < 500)
            {
                this.onClientErrorStatus?.Invoke(message);
                return;
            }

            this.onServerErrorStatus?.Invoke(message);
        }
    }
}