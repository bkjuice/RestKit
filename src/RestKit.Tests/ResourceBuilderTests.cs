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
    public class ResourceBuilderTests
    {
        [TestMethod]
        public void AsProtoResourceReturnsValidResourceInstance()
        {
            Resource.As(
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
            
            Resource.As(
                new HttpClient(handler), 
                "application/protobuf",
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void AsProtoResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.As(
                new HttpClient(handler),
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Post(new Uri("http://nowhere.com"), new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.As(
                new HttpClient(handler),
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Put(new Uri("http://nowhere.com"),new SimpleItem { Value = "a test value" });
        }

        [TestMethod]
        public void AsProtoResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.As(
                new HttpClient(handler),
                "application/protobuf", 
                ProtoSerializer.Serialize,
                (s, t) => ProtoSerializer.Deserialize<SimpleItem>(s))
            .Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void XmlReturnsValidResourceInstance()
        {
            Resource.Xml().Should().NotBeNull();
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

            Resource.Xml(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void XmlResourceCanBeDeserializedAsXml()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("<SimpleItem><Value>test</Value></SimpleItem>", Encoding.UTF8, "text/xml"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            // TODO: GET AND DELETE, no type parameter!
            var representation = Resource.Xml(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));
            var result = representation.Deserialize<SimpleItem>();
            result.Value.Should().Be("test");
        }

        [TestMethod]
        public void XmlResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));

            // This should just be XElement or XDocument:
            Resource.Xml(new HttpClient(handler)).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Xml(new HttpClient(handler)).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void XmlResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Xml(new HttpClient(handler)).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonReturnsValidResourceInstance()
        {
            Resource.Json().Should().NotBeNull();
        }

        [TestMethod]
        public void JsonResourceInvokesGet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("{}"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            // TODO: Prove invocation happens:
            Resource.Json(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesDelete()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Json(new HttpClient(handler)).Delete(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void JsonResourceInvokesPost()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Json(new HttpClient(handler)).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceInvokesPut()
        {
            // TODO: Prove invocation happens:
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Json(new HttpClient(handler)).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void JsonResourceDeserializesWithDataMemberMarkup()
        {
            var content = new StreamContent("{ \"value\":\"test\"}".CreateStream());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: content,
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            var representation = Resource.Json(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));

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
            Resource.Text(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));
        }

        [TestMethod]
        public void TextRepresentationCanBeReadAsString()
        {
            var handler = HttpStatusCode.OK.BuildHandler(
                expectedContent: new StringContent("result"),
                requestCallback: (r) => r.Method.Should().Be(HttpMethod.Get));

            var representation = Resource.Text(new HttpClient(handler)).Get(new Uri("http://nowhere.com"));
            representation.GetContentAsText().Should().Be("result");
        }

        [TestMethod]
        public void TextResourceInvokesPost()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Post));
            Resource.Text(new HttpClient(handler)).Post(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesPut()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Put));
            Resource.Text(new HttpClient(handler)).Put(new Uri("http://nowhere.com"), "test");
        }

        [TestMethod]
        public void TextResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Text(new HttpClient(handler)).Delete(new Uri("http://nowhere.com"));
        }
    }
}
