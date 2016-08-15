using System;
using System.IO;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestKit.Tests.Content;

namespace RestKit.Tests
{
    [TestClass]
    public class ResourceTests
    {
        [TestMethod]
        public void AsProtoResourceReturnsValidResourceInstance()
        {
            Resource.As(
                "application/protobuf", // whatever it is...
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Should().NotBeNull();
        }

        [TestMethod]
        public void AsProtoResourceInvokesGet()
        {
            var content = new MemoryStream();
            var item = new SimpleItem { Value = "a test value" };
            ProtoSerializer.Serialize(item, content);
            content.Position = 0;

            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StreamContent(content),
                assert: (r, c) => r.Method.Should().Be(HttpMethod.Get));
            
            Resource.As(
                "application/protobuf",
                handler.Object, 
                ProtoSerializer.Serialize, 
                ProtoSerializer.Deserialize<SimpleItem>)
            .Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void AsProtoResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Post));
            Resource.As(
                "application/protobuf", 
                handler.Object,
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Post(new Uri("http://nowhere.com"), new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Put));
            Resource.As(
                "application/protobuf", 
                handler.Object,
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Put(new Uri("http://nowhere.com"),new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.As(
                "application/protobuf", 
                handler.Object,
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void XmlReturnsValidResourceInstance()
        {
            Resource.Xml<SimpleItem>().Should().NotBeNull();
        }

        [TestMethod]
        public void XmlResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("<SimpleItem><Value>test</Value></SimpleItem>"),
                assert: (r, c) => r.Method.Should().Be(HttpMethod.Get));
            Resource.Xml<SimpleItem>(handler.Object).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void XmlResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Xml<string>(handler.Object).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Xml<string>(handler.Object).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Xml<string>(handler.Object).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonReturnsValidResourceInstance()
        {
            Resource.Json<SimpleItem>().Should().NotBeNull();
        }

        [TestMethod]
        public void JsonResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("{}"),
                assert: (r, c) => r.Method.Should().Be(HttpMethod.Get));
            Resource.Json<string>(handler.Object).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Json<string>(handler.Object).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Json<string>(handler.Object).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Json<string>(handler.Object).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void TextReturnsValidResourceInstance()
        {
            Resource.Text().Should().NotBeNull();
        }

        [TestMethod]
        public void TextResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("result"),
                assert: (r, c) => r.Method.Should().Be(HttpMethod.Get));
            Resource.Text(handler.Object).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void TextResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Text(handler.Object).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Text(handler.Object).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(assert: (r, c) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Text(handler.Object).Delete(new Uri("http://nowhere.com"));
        }
    }
}
