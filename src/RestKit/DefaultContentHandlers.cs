using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace RestKit
{
    internal static class DefaultContentHandlers
    {
        private static Dictionary<Type, Func<object, HttpContent>> writers =
            new Dictionary<Type, Func<object, HttpContent>>
            {
                [typeof(string)] = o => new StringContent((string)o, Encoding.UTF8),
                [typeof(Stream)] = o => ((Stream)o).CopyToContent(),
                [typeof(MemoryStream)] = o => ((Stream)o).CopyToContent(),
                [typeof(FileStream)] = o => ((Stream)o).CopyToContent(),
                [typeof(XDocument)] = o => ((XDocument)o).CopyToContent(),
                [typeof(XElement)] = o => ((XElement)o).CopyToContent(),
                [typeof(StreamReader)] = o => (((TextReader)o).CopyToContent()),
                [typeof(StringReader)] = o => (((TextReader)o).CopyToContent()),
                [typeof(TextReader)] = o => (((TextReader)o).CopyToContent()),
            };

        private static HttpContent CopyToContent(this StringBuilder builder)
        {
            return new StringContent(builder.ToString(), Encoding.UTF8);
        }

        private static HttpContent CopyToContent(this TextReader reader)
        {
            return new StringContent(reader.ReadToEnd(), Encoding.UTF8);
        }

        private static HttpContent CopyToContent(this XNode node)
        {
            var content = new MemoryStream(4096);
            var writer = new XmlTextWriter(content, Encoding.UTF8);
            node.WriteTo(writer);
            writer.Flush();
            content.Position = 0;
            return new StreamContent(content);
        }

        private static HttpContent CopyToContent(this Stream s)
        {
            // Ensure the stream is wholly owned to
            // avoid surprises with disposals:
            var content = new MemoryStream(4096);
            s.CopyTo(content);
            content.Position = 0;
            return new StreamContent(content);
        }
    }
}
