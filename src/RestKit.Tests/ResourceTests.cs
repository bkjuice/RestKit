using System;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestKit.TestLib;

namespace RestKit.Tests
{
    [TestClass]
    public class ResourceTests
    {
        private static readonly Uri DummyUri = new Uri("http://nowhere.com");

        [TestMethod]
        public void ResourceWillPassBufferSettingToRepresentation()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub(doNotBuffer: true);
            var representation = resource.Get(new Uri("http://nowhere.com"));
            representation.Buffered.Should().BeFalse();
        }

        [TestMethod]
        public void ResourceConfigIsNotNullOnGet()
        {
            new Resource().Events.Should().NotBeNull();
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
            Action test = () => new Resource(handler).Post(DummyUri, "test");
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
            Action test = () => new Resource(handler).Put(DummyUri, "test");
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
        public void CancelPendingRequestsDoesNotThrowWhenInitializedWithExplicitClient()
        {
            Action test = () => new Resource(new HttpClient()).CancelPendingRequests();
            test.ShouldNotThrow();
        }

        [TestMethod]
        public void CancelPendingRequestsThrowsWhenInitializedForPooledClient()
        {
            Action test = () => new Resource().CancelPendingRequests();
            test.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void OnBeforeGetIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnBeforeGet(c => invoked = true);
            resource.Get(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeGetIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnBeforeGet(c => invoked1 = true);
            resource.Events.OnBeforeGet(c => invoked2 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnBeforePost((c, d) => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnBeforePost((c, d) => invoked1 = true);
            resource.Events.OnBeforePost((c, d) => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnBeforePut((c, d) => invoked = true);
            resource.Put(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnBeforePut((c, d) => invoked1 = true);
            resource.Events.OnBeforePut((c, d) => invoked2 = true);
            resource.Put(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnBeforeDelete(c => invoked = true);
            resource.Delete(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnBeforeDelete(c => invoked1 = true);
            resource.Events.OnBeforeDelete(c => invoked2 = true);
            resource.Delete(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsInvokedForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            resource.Events.OnReply((m) => invoked1 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsMulticastForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnReply((m) => invoked1 = true);
            resource.Events.OnReply((m) => invoked2 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyStatusCodeIsOKForGetIfOKIsReturned()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var result = resource.Get(DummyUri);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public void OnReplyStatusReasonIsOKForGetIfOKIsReturned()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var result = resource.Get(DummyUri);
            result.ReasonPhrase.Should().Be("OK");
        }

        [TestMethod]
        public void OnInformationalStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.Continue.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnInformationStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnInformationalStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Continue.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnInformationStatus(r => invoked1 = true);
            resource.Events.OnInformationStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnSuccessStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnSuccessStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnSuccessStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Created.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnSuccessStatus(r => invoked1 = true);
            resource.Events.OnSuccessStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnRedirectStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.Moved.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnRedirectStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnRedirectStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.MovedPermanently.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnRedirectStatus(r => invoked1 = true);
            resource.Events.OnRedirectStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnClientErrorStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.BadRequest.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnClientErrorStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnClientErrorStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Unauthorized.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnClientErrorStatus(r => invoked1 = true);
            resource.Events.OnClientErrorStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnServerErrorStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.Events.OnServerErrorStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnServerErrorStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.BadGateway.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnServerErrorStatus(r => invoked1 = true);
            resource.Events.OnServerErrorStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnContentMismatchIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub(new StringContent ("<root/>", Encoding.UTF8, "application/xml"));
            var invoked = false;
            resource.Events.OnContentMismatch(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnContentMismatchIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.Events.OnContentMismatch(r => invoked1 = true);
            resource.Events.OnContentMismatch(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }
    }
}