using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace RestKit
{
    public static class Resource
    {
        public const string ApplicationJson = "application/json";

        public const string TextXml = "text/xml";

        public const string TextPlain = "text/plain";

        public static void SetGlobalMinSecurityProtocol(SecurityProtocolType protocolKind)
        {
            ServicePointManager.SecurityProtocol = protocolKind;
        }

        public static IHttpResource<T> Json<T>()
        {
            return ConfigureJson(new Resource<T>());
        }

        public static IHttpResource<T> Json<T>(HttpMessageHandler handler)
        {
            return ConfigureJson(new Resource<T>(handler));
        }

        public static IHttpResource<T> Xml<T>()
        {
            return ConfigureXml(new Resource<T>());
        }

        public static IHttpResource<T> Xml<T>(HttpMessageHandler handler)
        {
            return ConfigureXml(new Resource<T>(handler));
        }

        public static IHttpResource<string> Text()
        {
            return ConfigureText(new Resource<string>());
        }

        public static IHttpResource<string> Text(HttpMessageHandler handler)
        {
            return ConfigureText(new Resource<string>(handler));
        }

        public static IHttpResource<T> As<T>(string mediaType, Action<T, Stream> serializer, Func<Stream, T> deserializer)
        {
            return Configure(new Resource<T>(), serializer, deserializer, mediaType);
        }

        public static IHttpResource<T> As<T>(string mediaType, HttpMessageHandler handler, Action<T, Stream> serializer, Func<Stream, T> deserializer)
        {
            return Configure(new Resource<T>(handler), serializer, deserializer, mediaType);
        }

        private static Resource<T> ConfigureJson<T>(Resource<T> resource)
        {
            return Configure(resource, SerializeJson, DeserializeJson<T>, ApplicationJson);
        }

        private static Resource<T> ConfigureXml<T>(Resource<T> resource)
        {
            return Configure(resource, SerializeXml, DeserializeXml<T>, TextXml);
        }

        private static Resource<string> ConfigureText(Resource<string> resource)
        {
            return Configure(
                resource,
                (s, io) => new StreamWriter(io).Write(s),
                (io) => new StreamReader(io).ReadToEnd(),
                TextPlain);
        }

        private static Resource<T> Configure<T>(Resource<T> provider, Action<T, Stream> serializer, Func<Stream, T> deserializer, string mediaType)
        {
            provider.AddDeserializer(deserializer, mediaType);
            provider.SetSerializer(serializer);
            provider.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return provider;
        }

        private static T DeserializeJson<T>(Stream json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var reader = new StreamReader(json))
            {
                return (T)serializer.ReadObject(json);
            }
        }

        private static void SerializeJson<T>(T resource, Stream output)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(output, resource);
        }

        private static T DeserializeXml<T>(Stream xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(xml);
        }

        private static void SerializeXml<T>(T resource, Stream output)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(output, resource);
        }
    }
}