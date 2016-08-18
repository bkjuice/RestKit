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

        public static IHttpResource Json()
        {
            // TODO: Allow for a deferred serializer for the body
            return ConfigureJson(new Resource<object>());
        }

        public static IHttpResource Json(HttpMessageHandler handler)
        {
            // TODO: Allow for a deferred serializer for the body
            return ConfigureJson(new Resource<object>(handler));
        }

        public static IHttpResource<TRequest> Json<TRequest>()
        {
            return ConfigureJson(new Resource<TRequest>());
        }

        public static IHttpResource<TRequest> Json<TRequest>(HttpMessageHandler handler)
        {
            return ConfigureJson(new Resource<TRequest>(handler));
        }

        public static IHttpResource<TRequest> Json<TRequest, TReply>()
        {
            return ConfigureJson<TRequest, TReply>(new Resource<TRequest>());
        }

        public static IHttpResource<TRequest> Json<TRequest, TReply>(HttpMessageHandler handler)
        {
            return ConfigureJson<TRequest, TReply>(new Resource<TRequest>(handler));
        }

        public static IHttpResource Xml()
        {
            // TODO: Allow for a deferred serializer for the body
            return ConfigureXml(new Resource<object>());
        }

        public static IHttpResource Xml(HttpMessageHandler handler)
        {
            // TODO: Allow for a deferred serializer for the body
            return ConfigureXml(new Resource<object>(handler));
        }

        public static IHttpResource<TRequest> Xml<TRequest>()
        {
            return ConfigureXml(new Resource<TRequest>());
        }

        public static IHttpResource<TRequest> Xml<TRequest>(HttpMessageHandler handler)
        {
            return ConfigureXml(new Resource<TRequest>(handler));
        }

        public static IHttpResource<TRequest> Xml<TRequest, TReply>()
        {
            return ConfigureXml<TRequest, TReply>(new Resource<TRequest>());
        }

        public static IHttpResource<TRequest> Xml<TRequest, TReply>(HttpMessageHandler handler)
        {
            return ConfigureXml<TRequest, TReply>(new Resource<TRequest>(handler));
        }

        public static IHttpResource<string> Text()
        {
            return ConfigureText(new Resource<string>());
        }

        public static IHttpResource<string> Text(HttpMessageHandler handler)
        {
            return ConfigureText(new Resource<string>(handler));
        }

        public static IHttpResource As<TReply>(string mediaType, Func<Stream, TReply> deserializer)
        {
            return Configure(new Resource<object>(), new MediaHandler<TReply>(deserializer, mediaType), mediaType);
        }

        public static IHttpResource As<TReply>(string mediaType, HttpMessageHandler handler, Func<Stream, TReply> deserializer)
        {
            return Configure(new Resource<object>(handler), new MediaHandler<TReply>(deserializer, mediaType), mediaType);
        }

        public static IHttpResource<TRequest> As<TRequest, TReply>(
            string mediaType,
            Action<TRequest, Stream> serializer,
            Func<Stream, TReply> deserializer)
        {
            return Configure(new Resource<TRequest>(), serializer, deserializer, mediaType);
        }

        public static IHttpResource<TRequest> As<TRequest, TReply>(
            string mediaType,
            HttpMessageHandler handler,
            Action<TRequest, Stream> serializer,
            Func<Stream, TReply> deserializer)
        {
            return Configure(new Resource<TRequest>(handler), serializer, deserializer, mediaType);
        }

        private static IHttpResource ConfigureJson(IHttpResource resource)
        {
            return Configure(resource, new MediaHandler(DeserializeJson, ApplicationJson), ApplicationJson);
        }

        private static IHttpResource<TRequest> ConfigureJson<TRequest>(IHttpResource<TRequest> resource)
        {
            return Configure(resource, SerializeJson, DeserializeJson, ApplicationJson);
        }

        private static IHttpResource<TRequest> ConfigureJson<TRequest, TReply>(IHttpResource<TRequest> resource)
        {
            return Configure(resource, SerializeJson, DeserializeJson<TReply>, ApplicationJson);
        }

        private static IHttpResource ConfigureXml(IHttpResource resource)
        {
            return Configure(resource, new MediaHandler(DeserializeXml, TextXml), TextXml);
        }

        private static IHttpResource<TRequest> ConfigureXml<TRequest>(IHttpResource<TRequest> resource)
        {
            return Configure(resource, SerializeXml, DeserializeXml, TextXml);
        }

        private static IHttpResource<TRequest> ConfigureXml<TRequest, TReply>(Resource<TRequest> resource)
        {
            return Configure(resource, SerializeXml, DeserializeXml<TReply>, TextXml);
        }

        private static IHttpResource<string> ConfigureText(Resource<string> resource)
        {
            return Configure(
                resource,
                (s, io) => new StreamWriter(io).Write(s),
                (io) => new StreamReader(io).ReadToEnd(),
                TextPlain);
        }

        private static IHttpResource Configure(
            IHttpResource resource,
            IMediaHandler handler,
            string mediaType)
        {
            resource.AddMediaDeserializer(handler);
            resource.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            return resource;
        }

        private static IHttpResource<TRequest> Configure<TRequest>(
          IHttpResource<TRequest> resource,
          Action<TRequest, Stream> serializer,
          Func<Stream, Type, object> deserializer,
          string mediaType)
        {
            resource.Body.SetSerializer(serializer);
            Configure(resource, new MediaHandler(deserializer, mediaType), mediaType);
            return resource;
        }

        private static IHttpResource<TRequest> Configure<TRequest, TReply>(
           IHttpResource<TRequest> resource,
           Action<TRequest, Stream> serializer,
           Func<Stream, TReply> deserializer,
           string mediaType)
        {
            resource.Body.SetSerializer(serializer);
            Configure(resource, new MediaHandler<TReply>(deserializer, mediaType), mediaType);
            return resource;
        }

        private static T DeserializeJson<T>(Stream json)
        {
            return (T)DeserializeJson(json, typeof(T));
        }

        private static object DeserializeJson(Stream json, Type t)
        {
            var serializer = new DataContractJsonSerializer(t);
            using (var reader = new StreamReader(json))
            {
                return serializer.ReadObject(json);
            }
        }

        private static void SerializeJson<T>(T resource, Stream output)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            serializer.WriteObject(output, resource);
        }

        private static T DeserializeXml<T>(Stream xml)
        {
            return (T)DeserializeXml(xml, (typeof(T)));
        }

        private static object DeserializeXml(Stream xml, Type t)
        {
            var serializer = new XmlSerializer(t);
            return serializer.Deserialize(xml);
        }

        private static void SerializeXml<T>(T resource, Stream output)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(output, resource);
        }
    }
}