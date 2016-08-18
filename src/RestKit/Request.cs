using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace RestKit
{
    public static class Request
    {
        public const string ApplicationJson = "application/json";

        public const string TextXml = "text/xml";

        public const string TextPlain = "text/plain";

        public static void SetGlobalMinSecurityProtocol(SecurityProtocolType protocolKind)
        {
            ServicePointManager.SecurityProtocol = protocolKind;
        }

        public static Resource Json(string mediaType = ApplicationJson)
        {
            return ConfigureJson(new Resource(), mediaType);
        }

        public static Resource Json(HttpMessageHandler handler, string mediaType = ApplicationJson)
        {
            return ConfigureJson(new Resource(handler), mediaType);
        }

        public static Resource Xml(string mediaType = TextXml)
        {
            return ConfigureXml(new Resource(), mediaType);
        }

        public static Resource Xml(HttpMessageHandler handler, string mediaType = TextXml)
        {
            return ConfigureXml(new Resource(handler), mediaType);
        }

        public static Resource Text(string mediaType = TextPlain)
        {
            return ConfigureText(new Resource(), mediaType);
        }

        public static Resource Text(HttpMessageHandler handler, string mediaType = TextPlain)
        {
            return ConfigureText(new Resource(handler), mediaType);
        }

        public static Resource As(string mediaType, Action<object, Stream> serializer, Func<Stream, Type, object> deserializer)
        {
            return ConfigureContent(new Resource(), serializer, deserializer, mediaType);
        }

        public static Resource As(HttpMessageHandler handler, string mediaType, Action<object, Stream> serializer, Func<Stream, Type, object> deserializer)
        {
            return ConfigureContent(new Resource(handler), serializer, deserializer, mediaType);
        }

        private static Resource ConfigureJson(Resource resource, string mediaType)
        {
            return ConfigureContent(resource, SerializeJson, DeserializeJson, mediaType);
        }

        private static Resource ConfigureXml(Resource resource, string mediaType)
        {
            return ConfigureContent(resource, SerializeXml, DeserializeXml, mediaType);
        }

        private static Resource ConfigureText(Resource resource, string mediaType)
        {
            return ConfigureContent(
                resource,
                (s, io) => new StreamWriter(io).Write(s),
                (io, t) => new StreamReader(io).ReadToEnd(),
                mediaType);
        }

        private static Resource ConfigureContent(
          Resource resource,
          Action<object, Stream> serializer,
          Func<Stream, Type, object> deserializer,
          string mediaType)
        {
            resource.SetMediaSerializer(serializer);
            resource.AddMediaDeserializer(deserializer, mediaType);
            resource.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return resource;
        }

        private static object DeserializeJson(Stream json, Type t)
        {
            Contract.Requires<ArgumentNullException>(t != null);
            Contract.Requires<ArgumentNullException>(json != null);

            var serializer = new DataContractJsonSerializer(t);
            using (var reader = new StreamReader(json))
            {
                return serializer.ReadObject(json);
            }
        }

        private static void SerializeJson(object resource, Stream output)
        {
            Contract.Requires<ArgumentNullException>(resource != null);
            Contract.Requires<ArgumentNullException>(output != null);

            var serializer = new DataContractJsonSerializer(resource.GetType());
            serializer.WriteObject(output, resource);
        }

        private static object DeserializeXml(Stream xml, Type t)
        {
            Contract.Requires<ArgumentNullException>(t != null);
            Contract.Requires<ArgumentNullException>(xml != null);

            var serializer = new XmlSerializer(t);
            return serializer.Deserialize(xml);
        }

        private static void SerializeXml(object resource, Stream output)
        {
            Contract.Requires<ArgumentNullException>(resource != null);
            Contract.Requires<ArgumentNullException>(output != null);

            var serializer = new XmlSerializer(resource.GetType());
            serializer.Serialize(output, resource);
        }
    }
}