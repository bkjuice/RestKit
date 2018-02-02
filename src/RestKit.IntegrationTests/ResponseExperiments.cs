using System;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.IntegrationTests
{
    [TestClass]
    public class ResponseExperiments
    {
        [TestMethod]
        public void RedirectForNonTLSGoogle()
        {
           Resource.Text().Get(new Uri("http://www.google.com"));
        }

        [TestMethod]
        public void UrlEchoGet500()
        {
            var result = Resource.Text().Get(new Uri("http://urlecho.appspot.com/echo?status=500"));
            result.IsUnexpectedMediaType.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }


    }
}
