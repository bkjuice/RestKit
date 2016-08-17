using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestKit.Tests
{
    [TestClass]
    public class RepresentationTests
    {
        [TestMethod]
        public void JsonContentAsDynamicWorksWithFlatJson()
        {
            //var response = new Response<>
        }


        private static Stream CreateContent(string data)
        {
            var s = new MemoryStream();
            using (var writer = new StreamWriter(s))
            {
                writer.Write(data);
                writer.Flush();
            }

            s.Position = 0;
            return s;
        } 
    }
}
