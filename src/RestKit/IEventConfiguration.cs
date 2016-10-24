using System;
using System.IO;
using System.Net.Http;

namespace RestKit
{
    public interface IEventConfiguration
    {
        void OnBeforeGet(Action<HttpClient> action);

        void OnBeforePost(Action<HttpClient, HttpContent> action);

        void OnBeforePut(Action<HttpClient, HttpContent> action);

        void OnBeforeDelete(Action<HttpClient> action);

        void OnReply(Action<HttpResponseMessage> action);

        void OnInformationStatus(Action<HttpResponseMessage> action);

        void OnSuccessStatus(Action<HttpResponseMessage> action);

        void OnRedirectStatus(Action<HttpResponseMessage> action);

        void OnClientErrorStatus(Action<HttpResponseMessage> action);

        void OnServerErrorStatus(Action<HttpResponseMessage> action);

        void OnContentMismatch(Action<HttpResponseMessage> action);
    }
}
