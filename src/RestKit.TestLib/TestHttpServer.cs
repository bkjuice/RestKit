using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestKit.TestLib
{
    public sealed class TestHttpServer : IDisposable
    {
        private readonly HttpServer _server;

        public TestHttpServer(string route = null, Action<HttpRequestMessage> assertions = null)
        {
            _server = GetServer(route ?? "test", assertions);
        }

        private static HttpServer GetServer(string route, Action<HttpRequestMessage> assertions)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: route);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MessageHandlers.Add(new TestResponseHandler(assertions));
            return new HttpServer(config);
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        private class TestResponseHandler : DelegatingHandler
        {
            private readonly Action<HttpRequestMessage> _assertions;
            public TestResponseHandler(Action<HttpRequestMessage> assertions)
            {
                _assertions = assertions;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                _assertions?.Invoke(request);
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
