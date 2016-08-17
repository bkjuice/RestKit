using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace RestKit
{
    internal static class ContentHandlers
    {
        ////private static Dictionary<Type, Func<object, HttpContent>> writers =
        ////    new Dictionary<Type, Func<object, HttpContent>>
        ////    {
        ////        [typeof(string)] = o => new StringContent((string)o, Encoding.UTF8),
        ////        [typeof(Stream)] = o => ((Stream)o).CopyToContent(),
        ////        [typeof(MemoryStream)] = o => ((Stream)o).CopyToContent(),
        ////        [typeof(FileStream)] = o => ((Stream)o).CopyToContent(),
        ////        [typeof(XDocument)] = o => ((XDocument)o).CopyToContent(),
        ////        [typeof(XElement)] = o => ((XElement)o).CopyToContent(),
        ////        [typeof(StreamReader)] = o => (((TextReader)o).CopyToContent()),
        ////        [typeof(StringReader)] = o => (((TextReader)o).CopyToContent()),
        ////        [typeof(TextReader)] = o => (((TextReader)o).CopyToContent()),
        ////    };

        public static ExpandoObject ToExpando(this IDictionary<string, object> input)
        {
            var expando = new ExpandoObject();
            var output = (IDictionary<string, object>)expando;

            foreach (var pair in input)
            {
                if (pair.Value is IDictionary<string, object>)
                {
                    output.Add(pair.Key, ((IDictionary<string, object>)pair.Value).ToExpando());
                }
                else if (pair.Value is ICollection)
                {
                    output.Add(pair.Key, ((ICollection)pair.Value).BuildExpandoArray());
                }
                else
                {
                    output.Add(pair);
                }
            }

            return expando;
        }

        private static List<object> BuildExpandoArray(this ICollection values)
        {
            var items = new List<object>();
            foreach (var item in values)
            {
                if (item is IDictionary<string, object>)
                {
                    items.Add(((IDictionary<string, object>)item).ToExpando());
                }
                else
                {
                    items.Add(item);
                }
            }

            return items;
        }

        ////private static HttpContent CopyToContent(this StringBuilder builder)
        ////{
        ////    return new StringContent(builder.ToString(), Encoding.UTF8);
        ////}

        ////private static HttpContent CopyToContent(this TextReader reader)
        ////{
        ////    return new StringContent(reader.ReadToEnd(), Encoding.UTF8);
        ////}

        ////private static HttpContent CopyToContent(this XNode node)
        ////{
        ////    var content = new MemoryStream(4096);
        ////    var writer = new XmlTextWriter(content, Encoding.UTF8);
        ////    node.WriteTo(writer);
        ////    writer.Flush();
        ////    content.Position = 0;
        ////    return new StreamContent(content);
        ////}

        ////private static HttpContent CopyToContent(this Stream s)
        ////{
        ////    // Ensure the stream is wholly owned to
        ////    // avoid surprises with disposals:
        ////    var content = new MemoryStream(4096);
        ////    s.CopyTo(content);
        ////    content.Position = 0;
        ////    return new StreamContent(content);
        ////}
    }
}
