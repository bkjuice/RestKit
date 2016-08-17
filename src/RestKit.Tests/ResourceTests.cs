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

            // http://stackoverflow.com/questions/4559991/any-way-to-make-datacontractjsonserializer-serialize-dictionaries-properly
            // http://stackoverflow.com/questions/14980973/deserialize-json-to-dictionary-with-datacontractjsonserializer
            // http://stackoverflow.com/questions/13631208/deserializing-a-json-object-hierarchy-into-a-hierarchy-of-dictionarystring-obj/13631324#13631324
            Resource.Json<string>(handler).Get(new Uri("http://nowhere.com"));
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
        public void JsonResourceInvokesDelete()
        {
            var handler = HttpStatusCode.OK.BuildHandler(requestCallback: (r) => r.Method.Should().Be(HttpMethod.Delete));
            Resource.Json<string>(handler).Delete(new Uri("http://nowhere.com"));
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
