using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace RestKit
{
    public sealed class Representation : IDisposable
    {
        private const int defaultBufferSize = 4096;

        private IMediaHandler mediaHandler;

        private Lazy<ReadOnlySeekableStream> contentCopy;

        public Representation(HttpResponseMessage reply, MediaChain mediaHandlers = null)
        {
            Contract.Requires<ArgumentNullException>(reply != null);

            this.Message = reply;
            this.contentCopy = new Lazy<ReadOnlySeekableStream>(this.InitializeRawContent);

            var media = reply.Content?.Headers?.ContentType?.MediaType;
            var accepts = reply.RequestMessage?.Headers?.Accept;
            this.mediaHandler = mediaHandlers?.GetHandlerFor(media);
            this.MediaType = media ?? string.Empty;
            this.IsUnexpectedMediaType = accepts?.FirstOrDefault(h => h.MediaType.Equals(media, StringComparison.OrdinalIgnoreCase)) == null;
            this.StatusCode = reply.StatusCode;
            this.ReasonPhrase = reply.ReasonPhrase;
        }

        public HttpResponseMessage Message { get; private set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public bool MayHaveContent
        {
            get
            {
                return
                    this.StatusCode != HttpStatusCode.NoContent &&
                    (
                        this.Message.Content?.Headers?.ContentLength > 0 ||
                        this.Message.Headers?.TransferEncodingChunked == true
                    );
            }
        }

        public bool IsUnexpectedMediaType { get; private set; }

        public string MediaType { get; private set; }

        public bool CanDeserialize
        {
            get
            {
                return this.mediaHandler != null;
            }
        }

        public void Dispose()
        {
            this.Message.Dispose();
            GC.SuppressFinalize(this);
        }

        public TReply Deserialize<TReply>()
        {
            if (this.mediaHandler == null)
            {
                throw new InvalidOperationException($"The representation cannot deserialize because there is no defined handler for media type '{ this.MediaType }'. Ensure a deserializer that handles this media type representation is added to the resource before executing the Http method.");
            }

            return (TReply)this.mediaHandler.Deserialize(this.GetContentAsStream());
        }

        public XElement GetContentAsXElement()
        {
            return XElement.Load(this.GetContentAsStream(), LoadOptions.PreserveWhitespace);
        }
        public XDocument GetContentAsXDocument()
        {
            return XDocument.Load(this.GetContentAsStream(), LoadOptions.PreserveWhitespace);
        }

        public dynamic GetContentAsXml(XmlConventions conventions = null)
        {
            return this.GetContentAsXElement().ToDynamic(conventions ?? XmlConventions.Default);
        }

        public XmlReader GetContentAsXmlReader()
        {
            return new XmlTextReader(this.GetContentAsStream());
        }

        public Dictionary<string, object> GetContentAsJsonMap()
        {
            return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(this.GetContentAsString());
        }

        public dynamic GetContentAsJson(CasingConvention casing = CasingConvention.AsIs)
        {
            return this.GetContentAsJsonMap().ToDynamic(casing);
        }

        public Stream GetContentAsStream()
        {
            this.contentCopy.Value.Position = 0;
            return this.contentCopy.Value;
        }

        public string GetContentAsString()
        {
            using (var reader = new StreamReader(this.GetContentAsStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private ReadOnlySeekableStream InitializeRawContent()
        {
            // TODO: Unwind this to allow async...forward only reading...
            var s = new MemoryStream(defaultBufferSize);
            this.Message.Content?.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult().CopyTo(s);
            return new ReadOnlySeekableStream(s);
        }
    }
}