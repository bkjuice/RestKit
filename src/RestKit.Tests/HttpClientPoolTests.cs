using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.Tests
{
    [TestClass]
    public class HttpClientPoolTests
    {
        [TestMethod]
        public void PooledClientIsSameReferenceForSameUri()
        {
            var pool = new HttpClientPool();
            var uri = new Uri("http://go.test.code.com/relativePath1");
            var client1 = pool.GetClient(uri, null);
            var client2 = pool.GetClient(uri, null);
            client1.Should().BeSameAs(client2);
        }

        [TestMethod]
        public void PooledClientIsSameReferenceForSameDnsHost()
        {
            var pool = new HttpClientPool();
            var uri1 = new Uri("http://go.test.code.com/relativePath1");
            var uri2 = new Uri("http://go.test.code.com/relativePath2");
            var client1 = pool.GetClient(uri1, null);
            var client2 = pool.GetClient(uri2, null);
            client1.Should().BeSameAs(client2);
        }

        [TestMethod]
        public void PooledClientIsNotSameReferenceForDifferentDnsHost()
        {
            var pool = new HttpClientPool();
            var uri1 = new Uri("http://go.test.code1.com/relativePath1");
            var uri2 = new Uri("http://go.test.code2.com/relativePath2");
            var client1 = pool.GetClient(uri1, null);
            var client2 = pool.GetClient(uri2, null);
            client1.Should().NotBeSameAs(client2);
        }

        [TestMethod]
        public void HttpClientPoolInvokesInitializationActionOnlyOncePerClient()
        {
            var pool = new HttpClientPool();
            var uri = new Uri("http://go.test.code.com/relativePath1");

            var count = 0;
            Action<HttpClient> initAction = c => count++;

            var client1 = pool.GetClient(uri, initAction);
            var client2 = pool.GetClient(uri, initAction);
            count.Should().Be(1);
        }

        [TestMethod]
        public void HttpClientPoolInvokesInitializationActionForEachClient()
        {
            var pool = new HttpClientPool();
            var uri1 = new Uri("http://go.test.code1.com/relativePath1");
            var uri2 = new Uri("http://go.test.code2.com/relativePath2");

            var count = 0;
            Action<HttpClient> initAction = c => count++;
            var client1 = pool.GetClient(uri1, initAction);
            var client2 = pool.GetClient(uri2, initAction);
            count.Should().Be(2);
        }
    }
}
