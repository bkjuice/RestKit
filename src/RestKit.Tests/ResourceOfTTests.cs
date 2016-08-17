using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public void ResourceConfigIsNotNullOnGet()
        {
            new Resource<string>().EventConfig.Should().NotBeNull();
        }

        [TestMethod]
        public void ResourceConfigReturnsValidHttpClientForDefaultCtor()
        {
            new Resource<string>().EventConfig.Client.Should().NotBeNull();
        }

        [TestMethod]
        public void ResourceConfigReturnsValidHttpClientForTestableCtor()
        {
            new Resource<string>(new Mock<HttpMessageHandler>().Object).EventConfig.Client.Should().NotBeNull();
        }

        ////[TestMethod]
        ////public void ResourceGetThrowsInvalidOperationExceptionWhenDeserializerIsNotSet()
        ////{
        ////    var handler = HttpStatusCode.OK.BuildHandler(new StringContent("test"));
        ////    Action test = () => new Resource<string>(handler).Get(DummyUri);
        ////    test.ShouldThrow<InvalidOperationException>();
        ////}

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
            Action test = () => new Resource<string>(handler).Post(DummyUri, "test");
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
            Action test = () => new Resource<string>(handler).Put(DummyUri, "test");
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
            resource.EventConfig.OnBeforeGet(c => invoked = true);
            resource.Get(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeGetIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnBeforeGet(c => invoked1 = true);
            resource.EventConfig.OnBeforeGet(c => invoked2 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnBeforePost((c, d) => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePostIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnBeforePost((c, d) => invoked1 = true);
            resource.EventConfig.OnBeforePost((c, d) => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnBeforePut((c, d) => invoked = true);
            resource.Put(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforePutIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnBeforePut((c, d) => invoked1 = true);
            resource.EventConfig.OnBeforePut((c, d) => invoked2 = true);
            resource.Put(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnBeforeDelete(c => invoked = true);
            resource.Delete(DummyUri);
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnBeforeDeleteIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnBeforeDelete(c => invoked1 = true);
            resource.EventConfig.OnBeforeDelete(c => invoked2 = true);
            resource.Delete(DummyUri);
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsInvokedForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            resource.EventConfig.OnReply((m) => invoked1 = true);
            resource.Get(DummyUri);
            invoked1.Should().BeTrue();
        }

        [TestMethod]
        public void OnReplyIsMulticastForGetIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnReply((m) => invoked1 = true);
            resource.EventConfig.OnReply((m) => invoked2 = true);
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
            resource.EventConfig.OnInformationStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnInformationalStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Continue.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnInformationStatus(r => invoked1 = true);
            resource.EventConfig.OnInformationStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnSuccessStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnSuccessStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnSuccessStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Created.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnSuccessStatus(r => invoked1 = true);
            resource.EventConfig.OnSuccessStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnRedirectStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.Moved.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnRedirectStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnRedirectStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.MovedPermanently.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnRedirectStatus(r => invoked1 = true);
            resource.EventConfig.OnRedirectStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnClientErrorStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.BadRequest.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnClientErrorStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnClientErrorStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.Unauthorized.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnClientErrorStatus(r => invoked1 = true);
            resource.EventConfig.OnClientErrorStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnServerErrorStatusIsInvokedIfSet()
        {
            var resource = HttpStatusCode.InternalServerError.SetupValidStringlyTypedStub();
            var invoked = false;
            resource.EventConfig.OnServerErrorStatus(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnServerErrorStatusIsMulticastIfSet()
        {
            var resource = HttpStatusCode.BadGateway.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnServerErrorStatus(r => invoked1 = true);
            resource.EventConfig.OnServerErrorStatus(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        [TestMethod]
        public void OnContentMismatchIsInvokedIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub(new StringContent ("<root/>", Encoding.UTF8, "application/xml"));
            var invoked = false;
            resource.EventConfig.OnContentMismatch(r => invoked = true);
            resource.Post(DummyUri, "test");
            invoked.Should().BeTrue();
        }

        [TestMethod]
        public void OnContentMismatchIsMulticastIfSet()
        {
            var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();
            var invoked1 = false;
            var invoked2 = false;
            resource.EventConfig.OnContentMismatch(r => invoked1 = true);
            resource.EventConfig.OnContentMismatch(r => invoked2 = true);
            resource.Post(DummyUri, "test");
            invoked1.Should().BeTrue();
            invoked2.Should().BeTrue();
        }

        ////[TestMethod]
        ////public void SetCanReadContentWillInvokeMethod()
        ////{
        ////    var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

        ////    var invoked = false;
        ////    resource.SetCanDeserialize(r => invoked = true);
        ////    var result = resource.Get(DummyUri);
        ////    invoked.Should().BeTrue();
        ////}

        ////[TestMethod]
        ////public void SetCanReadContentWillInvokeLastSetMethod()
        ////{
        ////    var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

        ////    var invoked1 = false;
        ////    var invoked2 = false;
        ////    resource.SetCanDeserialize(r => invoked1 = true);
        ////    resource.SetCanDeserialize(r => invoked2 = true);
        ////    var result = resource.Get(DummyUri);
        ////    invoked1.Should().BeFalse();
        ////    invoked2.Should().BeTrue();
        ////}

        ////[TestMethod]
        ////public void SetCanReadContentIgnoresNullMethod()
        ////{
        ////    var resource = HttpStatusCode.OK.SetupValidStringlyTypedStub();

        ////    var invoked1 = false;
        ////    var invoked2 = false;
        ////    resource.SetCanDeserialize(r => invoked1 = true);
        ////    resource.SetCanDeserialize(null);
        ////    var result = resource.Get(DummyUri);
        ////    invoked1.Should().BeTrue();
        ////    invoked2.Should().BeFalse();
        ////}
    }
}