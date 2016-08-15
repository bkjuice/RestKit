using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace RestKit.Tests
{
    public static class HandlerStubs
    {
        public static Mock<HttpMessageHandler> BuildHandler(
            this HttpStatusCode expectedStatus,
            HttpContent expectedContent = null,
            Action<HttpRequestMessage, CancellationToken> assert = null)
        {
            var handler = new Mock<HttpMessageHandler>();

            handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(
                Task.FromResult(new HttpResponseMessage(expectedStatus) { Content = expectedContent ?? new StringContent(string.Empty) }))
            .Callback<HttpRequestMessage, CancellationToken>(
                (r, c) =>
                {
                    assert?.Invoke(r, c);
                });

            return handler;
        }

        public static Resource<string> SetupValidStringlyTypedStub(this HttpStatusCode expectedStatus, string content = "test content")
        {
            var handler = expectedStatus.BuildHandler(new StringContent(content));
            var resource = new Resource<string>(handler.Object);
            resource.SetDeserializer(s => new StreamReader(s).ReadToEnd());
            resource.SetSerializer((s, io) => new StreamWriter(io).Write(s));
            return resource;
        }
    }
}
