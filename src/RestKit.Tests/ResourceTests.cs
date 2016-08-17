using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));
            
            Resource.As(
                "application/protobuf",
                handler, 
                ProtoSerializer.Serialize, 
                ProtoSerializer.Deserialize<SimpleItem>)
            .Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void AsProtoResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.As(
                "application/protobuf", 
                handler,
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Post(new Uri("http://nowhere.com"), new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.As(
                "application/protobuf", 
                handler,
                ProtoSerializer.Serialize,
                ProtoSerializer.Deserialize<SimpleItem>)
            .Put(new Uri("http://nowhere.com"),new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.As(
                "application/protobuf", 
                handler,
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
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            Resource.Xml<SimpleItem>(handler).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void XmlResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));

            // This should just be XElement or XDocument:
            Resource.Xml<string>(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Xml<string>(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Xml<string>(handler).Delete(new Uri("http://nowhere.com"));
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
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            // TODO: Type qualifier for get and delete make no sense...
            Resource.Json<string>(handler).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Json<string>(handler).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Json<string>(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Json<string>(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceDeserializesWithDataMemberMarkup()
        {
            var content = new StreamContent("{ \"value\":\"test\"}".CreateStream());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: content,
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            var representation = Resource.Json<SimpleItem>(handler).Get(new Uri("http://nowhere.com"));

            representation.CanDeserialize.Should().BeTrue();
            var data = representation.Deserialize<SimpleItem>();
            data.Value.Should().Be("test");
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
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));
            Resource.Text(handler).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void TextResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Text(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Text(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Text(handler).Delete(new Uri("http://nowhere.com"));
        }
    }
}
