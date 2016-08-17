using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.Tests
{
    [TestClass]
    public class MediaChainTests
    {
        [TestMethod]
        public void MediaChainIgnoresNullHandlers()
        {
            var chain = new MediaChain();
            chain.AddHandler(null);
            chain.AddHandler(null);
            chain.AddHandler(null);

            Action test = () => { chain.GetHandlerFor("test").Should().BeNull(); };
            test.ShouldNotThrow();
        }
    }
}
