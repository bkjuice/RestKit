using System;
using System.IO;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RestKit.Tests
{
    [TestClass]
    public class ResourceOfTTests
    {
        private static readonly Uri DummyUri = new Uri("http://nowhere.com");

        [TestMethod]
        public void ResourceConfigIsNotNullByDefault()
        {
            new Resource<string>().Config.Should().NotBeNull();
        }

        [TestMethod]
        public void ResourceConfigReturnsValidHttpClientForDefaultCtor()
        {
            new Resource<string>().Config.Client.Should().NotBeNull();
        }

        [TestMethod]
        public void ResourceConfigReturnsValidHttpClientForTestableCtor()
        {
            new Resource<string>(new Mock<HttpMessageHandler>().Object).Config.Client.Should().NotBeNull();
        }

        [TestMethod]
        public void ResourceGetThrowsInvalidOperationExceptionWhenDeserializerIsNotSet()
        {
            var handler = HttpStatusCode.OK.BuildHandler(new StringContent("test"));
            Action test = () => new Resource<string>(handler.Object).Get(DummyUri);
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ResourceGetDoesNotThrowInvalidOperationExceptionWhenDeserializerIsSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            Action test = () => resource.Get(DummyUri);
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ResourcePostThrowsInvalidOperationExceptionWhenSerializerIsNotSet()
        {
            var handler = HttpStatusCode.OK.BuildHandler();
            Action test = () => new Resource<string>(handler.Object).Post(DummyUri, "test");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ResourcePostDoesNotThrowInvalidOperationExceptionWhenSerializerIsSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            Action test = () => resource.Post(DummyUri, "test");
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ResourcePutThrowsInvalidOperationExceptionWhenSerializerIsNotSet()
        {
            var handler = HttpStatusCode.OK.BuildHandler();
            Action test = () => new Resource<string>(handler.Object).Put(DummyUri, "test");
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void ResourcePutDoesNotThrowInvalidOperationExceptionWhenSerializerIsSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            Action test = () => resource.Put(DummyUri, "test");
            test.ShouldNotThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void CancelPendingRequestsDoesNotThrowByDefault()
        {
            Action test = () => new Resource<string>().CancelPendingRequests();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void OnBeforeGetIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Config.OnBeforeGet(c => invoked = true);
            resource.Get(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeGetIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Config.OnBeforeGet(c => invoked1 = true);
            resource.Config.OnBeforeGet(c => invoked2 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Config.OnBeforePost((c, d) => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Config.OnBeforePost((c, d) => invoked1 = true);
            resource.Config.OnBeforePost((c, d) => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Config.OnBeforePut((c, d) => invoked = true);
            resource.Put(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Config.OnBeforePut((c, d) => invoked1 = true);
            resource.Config.OnBeforePut((c, d) => invoked2 = true);
            resource.Put(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Config.OnBeforeDelete(c => invoked = true);
            resource.Delete(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Config.OnBeforeDelete(c => invoked1 = true);
            resource.Config.OnBeforeDelete(c => invoked2 = true);
            resource.Delete(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsInvokedForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            resource.Config.OnReply((m) => invoked1 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsMulticastForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Config.OnReply((m) => invoked1 = true);
            resource.Config.OnReply((m) => invoked2 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyStatusCodeIsOKForGetIfOKIsReturned()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var result = resource.Get(DummyUri);
            result.Status.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void OnReplyStatusReasonIsOKForGetIfOKIsReturned()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var result = resource.Get(DummyUri);
            result.Status.StatusReason.Should().Be("OK");
        }

        [TestMethod]
        public void SetCanReadContentWillInvokeMethod()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

            var invoked = false;
            resource.SetCanDeserialize(r => invoked = true);
            var result = resource.Get(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void SetCanReadContentWillInvokeLastSetMethod()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

            var invoked1 = false;
            var invoked2 = false;
            resource.SetCanDeserialize(r => invoked1 = true);
            resource.SetCanDeserialize(r => invoked2 = true);
            var result = resource.Get(DummyUri);
            invoked1.Should().BeFalse();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void SetCanReadContentIgnoresNullMethod()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

            var invoked1 = false;
            var invoked2 = false;
            resource.SetCanDeserialize(r => invoked1 = true);
            resource.SetCanDeserialize(null);
            var result = resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeFalse();
        }
    }
}