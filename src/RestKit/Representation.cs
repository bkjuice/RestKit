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

        private MediaHandler mediaHandler;

        private Lazy<Task<ReadOnlySeekableStream>> contentCopy;

        public Representation(HttpResponseMessage reply, MediaChain mediaHandlers = null)
        {
            Contract.Requires<ArgumentNullException>(reply != null);

            this.Message = reply;
            this.contentCopy = new Lazy<Task<ReadOnlySeekableStream>>(this.InitializeRawContent);

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

        public bool HasNoContent
        {
            get
            {
                return
                    this.StatusCode == HttpStatusCode.NoContent ||
                    !(
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
            return this.DeserializeAsync<TReply>().Result;
        }

        public dynamic GetContentAsXml(XmlConventions conventions = null)
        {
            return this.GetContentAsXmlAsync(conventions).Result;
        }

        public XElement GetContentAsXElement()
        {
            return this.GetContentAsXElementAsync().Result;
        }

        public XDocument GetContentAsXDocument()
        {
            return this.GetContentAsXDocumentAsync().Result;
        }

        public XmlReader GetContentAsXmlReader()
        {
            return this.GetContentAsXmlReaderAsync().Result;
        }

        public Dictionary<string, object> GetContentAsJsonMap()
        {
            return this.GetContentAsJsonMapAsync().Result;
        }

        public dynamic GetContentAsJson(CasingConvention casing = CasingConvention.AsIs)
        {
            return this.GetContentAsJsonAsync(casing).Result;
        }

        public Stream GetContentAsStream()
        {
            return this.GetContentAsStreamAsync().Result;
        }

        public string GetContentAsText()
        {
            return this.GetContentAsTextAsync().Result;
        }

        public async Task<TReply> DeserializeAsync<TReply>()
        {
            if (this.mediaHandler == null)
            {
                throw new InvalidOperationException($"The representation cannot deserialize because there is no defined handler for media type '{ this.MediaType }'. Ensure a deserializer that handles this media type representation is added to the resource before executing the Http method.");
            }

            return (TReply)this.mediaHandler.Deserialize(await this.GetContentAsStreamAsync(), typeof(TReply));
        }
        
        public async Task<dynamic> GetContentAsXmlAsync(XmlConventions conventions = null)
        {
            return (await this.GetContentAsXElementAsync()).ToDynamic(conventions ?? XmlConventions.Default);
        }

        public async Task<XElement> GetContentAsXElementAsync()
        {
            return XElement.Load(await this.GetContentAsStreamAsync(), LoadOptions.PreserveWhitespace);
        }

        public async Task<XDocument> GetContentAsXDocumentAsync()
        {
            return XDocument.Load(await this.GetContentAsStreamAsync(), LoadOptions.PreserveWhitespace);
        }

        public async Task<XmlReader> GetContentAsXmlReaderAsync()
        {
            return new XmlTextReader(await this.GetContentAsStreamAsync());
        }

        public async Task<Dictionary<string, object>> GetContentAsJsonMapAsync()
        {
            return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(await this.GetContentAsTextAsync());
        }

        public async Task<dynamic> GetContentAsJsonAsync(CasingConvention casing = CasingConvention.AsIs)
        {
            return (await this.GetContentAsJsonMapAsync()).ToDynamic(casing);
        }

        public async Task<Stream> GetContentAsStreamAsync()
        {
            var copy = await this.contentCopy.Value;
            copy.Position = 0;
            return copy;
        }

        public async Task<string> GetContentAsTextAsync()
        {
            using (var reader = new StreamReader(await this.GetContentAsStreamAsync()))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task<ReadOnlySeekableStream> InitializeRawContent()
        {
            // TODO: Unwind this to allow async...forward only reading...
            var s = new MemoryStream(defaultBufferSize);
            if (this.Message?.Content != null)
            {
                var source = await this.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
                source.CopyTo(s);
            }

            return new ReadOnlySeekableStream(s);
        }
    }
}