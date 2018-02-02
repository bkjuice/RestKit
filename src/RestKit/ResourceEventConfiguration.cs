using System;
using System.Diagnostics.Contracts;
using System.Net.Http;

namespace RestKit
{
    public class ResourceEventConfiguration : IEventConfiguration
    {
        private readonly Func<Uri, HttpClient> clientPromise;

        private Action<HttpClient> onBeforeAction;

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

        public ResourceEventConfiguration(Func<Uri, HttpClient> clientPromise)
        {
            Contract.Requires<ArgumentNullException>(clientPromise != null);
            this.clientPromise = clientPromise;
        }

        public void OnBeforeAction(Action<HttpClient> action)
        {
            this.onBeforeAction += action;
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

        public void InvokeOnBeforeAction(Uri uri)
        {
            this.onBeforeAction?.Invoke(this.clientPromise(uri));
        }

        public void InvokeOnBeforeGet(Uri uri)
        {
            this.onBeforeGet?.Invoke(this.clientPromise(uri));
        }

        public void InvokeOnBeforePost(HttpContent content, Uri uri)
        {
            this.onBeforePost?.Invoke(this.clientPromise(uri), content);
        }

        public void InvokeOnBeforePut(HttpContent content, Uri uri)
        {
            this.onBeforePut?.Invoke(this.clientPromise(uri), content);
        }

        public void InvokeOnBeforeDelete(Uri uri)
        {
            this.onBeforeDelete?.Invoke(this.clientPromise(uri));
        }

        public void InvokeReplyActions(Representation representation)
        {
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