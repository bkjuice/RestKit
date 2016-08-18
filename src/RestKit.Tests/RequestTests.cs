using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestKit.Tests.Content;

namespace RestKit.Tests
{
    [TestClass]
    public class RequestTests
    {
        [TestMethod]
        public void AsProtoResourceReturnsValidResourceInstance()
        {
            Request.As(
                "application/protobuf", // whatever it is...
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
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
            
            Request.As(
                handler, 
                "application/protobuf",
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void AsProtoResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Request.As(
                handler,
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Post(new Uri("http://nowhere.com"), new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Request.As(
                handler,
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Put(new Uri("http://nowhere.com"),new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Request.As(
                handler,
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void XmlReturnsValidResourceInstance()
        {
            Request.Xml().Should().NotBeNull();
        }

        [TestMethod]
        public void XmlResourceInvokesGet()
        {
            var invoked = false;
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("<SimpleItem><Value>test</Value></SimpleItem>"),
                requestCallback: 
                (r) => {
                    invoked = true;
                    r.Method.Should().Be(HttpMethod.Get);
                });

            Request.Xml(handler).Get(new Uri("http://nowhere.com"));
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void XmlResourceCanBeDeserializedAsXml()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("<SimpleItem><Value>test</Value></SimpleItem>", Encoding.UTF8, "text/xml"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            // TODO: GET AND DELETE, no type parameter!
            var representation = Request.Xml(handler).Get(new Uri("http://nowhere.com"));
            var result = representation.Deserialize<SimpleItem>();
            result.Value.Should().Be("test");
        }

        [TestMethod]
        public void XmlResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));

            // This should just be XElement or XDocument:
            Request.Xml(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Request.Xml(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Request.Xml(handler).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonReturnsValidResourceInstance()
        {
            Request.Json().Should().NotBeNull();
        }

        [TestMethod]
        public void JsonResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("{}"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            // TODO: Prove invocation happens:
            Request.Json(handler).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesDelete()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Request.Json(handler).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesPost()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Request.Json(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceInvokesPut()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Request.Json(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceDeserializesWithDataMemberMarkup()
        {
            var content = new StreamContent("{ \"value\":\"test\"}".CreateStream());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: content,
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            var representation = Request.Json(handler).Get(new Uri("http://nowhere.com"));

            representation.CanDeserialize.Should().BeTrue();
            var data = representation.Deserialize<SimpleItem>();
            data.Value.Should().Be("test");
        }

        [TestMethod]
        public void TextReturnsValidResourceInstance()
        {
            Request.Text().Should().NotBeNull();
        }

        [TestMethod]
        public void TextResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("result"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));
            Request.Text(handler).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void TextRepresentationCanBeReadAsString()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("result"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            var representation = Request.Text(handler).Get(new Uri("http://nowhere.com"));
            representation.GetContentAsString().Should().Be("result");
        }

        [TestMethod]
        public void TextResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Request.Text(handler).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Request.Text(handler).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Request.Text(handler).Delete(new Uri("http://nowhere.com"));
        }
    }
}
