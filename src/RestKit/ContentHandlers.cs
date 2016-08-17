﻿using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace RestKit
{
    internal static class ContentHandlers
    {
        public static dynamic ToDynamic(this XElement node, XmlConventions conventions)
        {
            if (!(node.HasAttributes || node.HasElements))
            {
                return node.Value;
            }

            var expando = new ExpandoObject();
            var bucket = (IDictionary<string, object>)expando;
            var casing = conventions.Casing;
            if (node.HasAttributes)
            {
                var attributes = new ExpandoObject();
                var attributeBucket = (IDictionary<string, object>)attributes;
                foreach (var attribute in node.Attributes())
                {
                    attributeBucket.Add(attribute.Name.LocalName.AsCase(casing), attribute.Value);
                }

                bucket.Add(conventions.AttributeContainerName, attributes);
            }

            if (node.HasElements)
            {
                var groups = node.Elements().GroupBy(x => x.Name.LocalName);
                foreach (var group in groups)
                {
                    if (group.Skip(1).Any())
                    {
                        var items = new List<object>();
                        items.AddRange(group.Select(e => e.ToDynamic(conventions)));
                        bucket.Add(group.Key.AsCase(casing) + conventions.GroupSuffix, items);
                    }
                    else
                    {
                        bucket.Add(group.Key.AsCase(casing), group.First().ToDynamic(conventions));
                    }
                }
            }
            else
            {
                bucket.Add(conventions.ComplexElementValueName, node.Value);
            }

            return expando;
        }

        public static dynamic ToDynamic(this IDictionary<string, object> input, CasingConvention casing)
        {
            var expando = new ExpandoObject();
            var bucket = (IDictionary<string, object>)expando;

            foreach (var pair in input)
            {
                var key = pair.Key.AsCase(casing);
                if (pair.Value is IDictionary<string, object>)
                {
                    bucket.Add(key, ((IDictionary<string, object>)pair.Value).ToDynamic(casing));
                }
                else if (pair.Value is ICollection)
                {
                    bucket.Add(key, ((ICollection)pair.Value).BuildExpandoJsonArray(casing));
                }
                else
                {
                    bucket.Add(key, pair.Value);
                }
            }

            return expando;
        }

        private static List<object> BuildExpandoJsonArray(this IEnumerable values, CasingConvention casing)
        {
            var items = new List<object>();
            foreach (var item in values)
            {
                if (item is IDictionary<string, object>)
                {
                    items.Add(((IDictionary<string, object>)item).ToDynamic(casing));
                }
                else
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private static string AsCase(this string value, CasingConvention casing)
        {
            switch (casing)
            {
                case CasingConvention.Pascalish:
                    return value.Casify(false);

                case CasingConvention.Camelish:
                    return value.Casify(true);

                default:
                    return value;
            }
        }

        private static string Casify(this string value, bool camel)
        {
            var chars = value.ToCharArray();
            if (chars.Length > 0)
            {
                chars[0] = camel ? char.ToLower(chars[0]) : char.ToUpper(chars[0]);
                value = new string(chars);
            }

            return value;
        }
    }
}
