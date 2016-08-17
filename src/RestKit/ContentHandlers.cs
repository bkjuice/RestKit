using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml.Linq;

namespace RestKit
{
    internal static class ContentHandlers
    {
        public static dynamic ToDynamic(this XElement node)
        {
            dynamic output = new ExpandoObject();

            output.Name = node.Name.LocalName;
            output.Namespace = node.Name.Namespace?.NamespaceName ?? string.Empty;
            output.Value = node.Value;
            output.HasAttributes = node.HasAttributes;

            if (node.HasAttributes)
            {
                output.Attributes = new List<KeyValuePair<string, string>>();
                foreach (var attribute in node.Attributes())
                {
                    var keyedValue = new KeyValuePair<string, string>(attribute.Name.LocalName, attribute.Value);
                    output.Attributes.Add(keyedValue);
                }
            }

            output.HasElements = node.HasElements;
            if (node.HasElements)
            {
                output.Elements = new List<dynamic>();
                foreach (var element in node.Elements())
                {
                    dynamic temp = element.ToDynamic();
                    output.Elements.Add(temp);
                }
            }

            return output;
        }

        public static dynamic ToDynamic(this IDictionary<string, object> input)
        {
            var expando = new ExpandoObject();
            var output = (IDictionary<string, object>)expando;

            foreach (var pair in input)
            {
                if (pair.Value is IDictionary<string, object>)
                {
                    output.Add(pair.Key, ((IDictionary<string, object>)pair.Value).ToDynamic());
                }
                else if (pair.Value is IEnumerable)
                {
                    output.Add(pair.Key, ((IEnumerable)pair.Value).BuildExpandoJsonArray());
                }
                else
                {
                    output.Add(pair);
                }
            }

            return expando;
        }

        private static List<object> BuildExpandoJsonArray(this IEnumerable values)
        {
            var items = new List<object>();
            foreach (var item in values)
            {
                if (item is IDictionary<string, object>)
                {
                    items.Add(((IDictionary<string, object>)item).ToDynamic());
                }
                else
                {
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
