using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            resource.AddMediaDeserializer(s => new StreamReader(s).ReadToEnd(), "text/plain");
            resource.SetSerializer((s, io) => new StreamWriter(io).Write(s));
            return resource;
        }

        public static HttpResponseMessage CreateResponseMessage(this HttpStatusCode status, Stream data, string mediaType, string acceptedType = null)
        {
            acceptedType = acceptedType ?? mediaType;
            var response = new HttpResponseMessage(status);
            response.Content = CreateContent(data, mediaType);
            response.RequestMessage = new HttpRequestMessage();
            response.RequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptedType));
            return response;
        }

        public static HttpContent CreateContent(this Stream data, string mediaType)
        {
            var content = new StreamContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            content.Headers.ContentLength = data.Length;
            return content;
        }

        public static Stream CreateStream(this string data)
        {
            var s = new MemoryStream();
            var writer = new StreamWriter(s);
            writer.Write(data);
            writer.Flush();

            s.Position = 0;
            return s;
        }

        private class StubHandler : HttpMessageHandler
        {
            public HttpStatusCode ExpectedStatus { get; set; }

            public HttpContent ExpectedContent { get; set; }

            public Action<HttpRequestMessage> RequestCallback { get; set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                this.RequestCallback?.Invoke(request);
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
