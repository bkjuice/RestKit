using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.IntegrationTests
{
    [TestClass]
    public class WindowsAuthTests
    {
        [TestMethod]
        public void UsingWindowsAuth_passes_auth_token()
        {
            // TODO: Set up an actual http listener and echo the FQDN back...or something like that.
            // The windows auth scenario is difficult to verify, because you have to use the HttpClientHandler.
            // For this test to run, you must set up a valid http endpoint that challenges for windows auth:
            Action test = () => Resource.UsingWindowsAuth().AsJson().Get(new Uri("http://??"));
            //// test.ShouldNotThrow();
            test.ShouldThrow<Exception>();
        }
    }
}
