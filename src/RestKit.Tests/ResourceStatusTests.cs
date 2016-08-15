using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace RestKit.Tests
{
    [TestClass]
    public class ResourceStatusTests
    {
        [TestMethod]
        public void ResourceStatusReasonIsEmptyByDefault()
        {
            new ResourceStatus().StatusReason.Should().BeEmpty();
        }

        [TestMethod]
        public void ResourceStatusIs0ByDefault()
        {
            ((int)new ResourceStatus().StatusCode).Should().Be(0);
        }
    }
}
