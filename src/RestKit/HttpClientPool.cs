using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace RestKit
{
    public class HttpClientPool
    {
        private readonly ConcurrentDictionary<string, HttpClient> pool = new ConcurrentDictionary<string, HttpClient>(StringComparer.OrdinalIgnoreCase);

        public HttpClient GetClient(Uri uri, Action<HttpClient> initializationAction)
        {
            var host = uri.DnsSafeHost;
            HttpClient value;
            if (pool.TryGetValue(host, out value))
            {
                return value;
            }

            value = new HttpClient();
            initializationAction?.Invoke(value);
            pool.TryAdd(host, value);
            return value;
        }
    }
}
