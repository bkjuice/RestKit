using System;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.Tests
{
    [TestClass]
    public class ResourceBehaviorTests
    {
        [TestMethod]
        public void ResourceGetReturnsStatus500OnStatus500()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var result  = resource.Get(new Uri("http://nowhere.com"));
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ResourcePostReturnsStatus500OnStatus500()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var result = resource.Post(new Uri("http://nowhere.com"), "test");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ResourcePutReturnsStatus500OnStatus500()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var result = resource.Put(new Uri("http://nowhere.com"), "test");
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ResourceDeleteReturnsStatus500OnStatus500()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var result = resource.Delete(new Uri("http://nowhere.com"));
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public void ResourceHasContentIsFalseFor204Status()
        {
            var resource = HttpStatusCode.NoContent.SetupValidStringlyTypedStub();
            var result = resource.Get(new Uri("http://nowhere.com"));
            result.MayHaveContent.Should().BeFalse();
        }

        [TestMethod]
        public void ResourceHasContentIsFalseFor200StatusWithEmptyContent()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub(string.Empty);
            var result = resource.Get(new Uri("http://nowhere.com"));
            result.MayHaveContent.Should().BeFalse();
        }
    }
}
