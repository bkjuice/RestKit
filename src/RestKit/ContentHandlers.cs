﻿using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace RestKit
{
    internal static class ContentHandlers
    {
        // TODO: Add support for the HAL specification....
        public static dynamic ToDynamic(this XElement node, XmlConventions conventions)
        {
            var expando = new ExpandoObject();
            var bucket = (IDictionary<string, object>)expando;

            if (!(node.HasAttributes || node.HasElements))
            {
                bucket.Add(conventions.ElementValueName, node.Value);
                return expando;
            }

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
                        bucket.Add(group.Key.AsCase(casing), items);
                    }
                    else
                    {
                        bucket.Add(group.Key.AsCase(casing), group.First().ToDynamic(conventions));
                    }
                }
            }
            else
            {
                bucket.Add(conventions.ElementValueName, node.Value);
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
                if (!TryAddAsNestedDictionary(bucket, key, pair.Value, casing) &&
                    !TryAddAsCollection(bucket, key, pair.Value, casing))
                {
                    bucket.Add(key, pair.Value);

                }
            }

            return expando;
        }

        private static bool TryAddAsNestedDictionary(IDictionary<string, object> bucket, string key, object value, CasingConvention casing)
        {
            var nested = value as IDictionary<string, object>;
            if (nested != null)
            {
                bucket.Add(key, nested.ToDynamic(casing));
                return true;
            }

            return false;
        }

        private static bool TryAddAsCollection(IDictionary<string, object> bucket, string key, object value, CasingConvention casing)
        {
            var collection = value as ICollection;
            if (collection != null)
            {
                bucket.Add(key, collection.BuildExpandoJsonArray(casing));
                return true;
            }

            return false;
        }

        private static List<object> BuildExpandoJsonArray(this IEnumerable values, CasingConvention casing)
        {
            var items = new List<object>();
            foreach (var item in values)
            {
                var nested = item as IDictionary<string, object>;
                if (nested != null)
                {
                    items.Add(nested.ToDynamic(casing));
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
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Intentionally naive:
            if (camel && char.IsLower(value[0]))
            {
                return value;
            }

            var chars = value.ToCharArray();
            chars[0] = camel ? char.ToLower(chars[0]) : char.ToUpper(chars[0]);
            return new string(chars);
        }
    }
}
