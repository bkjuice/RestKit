using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace RestKit.Tests
{
    public static class HandlerStubs
    {
        public static HttpMessageHandler BuildHandler(
            this HttpStatusCode expectedStatus,
            HttpContent expectedContent = null,
            Action<HttpRequestMessage> requestCallback = null)
        {
            var handler = new StubHandler();
            handler.ExpectedStatus = expectedStatus;
            handler.ExpectedContent = expectedContent;
            handler.RequestCallback = requestCallback;
            return handler;
        }

        public static Resource<string> SetupValidStringlyTypedStub(this HttpStatusCode expectedStatus, string content = "test content")
        {
            return expectedStatus.SetupValidStringlyTypedStub(new StringContent(content, Encoding.UTF8, "text/plain"));
        }

        public static Resource<string> SetupValidStringlyTypedStub(this HttpStatusCode expectedStatus, HttpContent content)
        {
            var handler = expectedStatus.BuildHandler(content);
            var resource = new Resource<string>(handler);
            resource.AddDeserializer(s => new StreamReader(s).ReadToEnd(), "text/plain");
            resource.SetSerializer((s, io) => new StreamWriter(io).Write(s));
            return resource;
        }

        private class StubHandler : HttpMessageHandler
        {
            public HttpStatusCode ExpectedStatus { get; set; }

            public HttpContent ExpectedContent { get; set; }

            public Action<HttpRequestMessage> RequestCallback { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(
                    new HttpResponseMessage(this.ExpectedStatus)
                    {
                        Content = this.ExpectedContent ?? new StringContent(string.Empty),
                        RequestMessage = request
                    });
            }
        }
    }
}
