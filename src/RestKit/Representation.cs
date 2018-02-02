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
        private const int DefaultBufferSize = 4096;

        private readonly MediaHandler mediaHandler;

        private readonly Lazy<Task<ReadOnlySeekableStream>> contentCopy;

        public Representation(HttpResponseMessage reply, bool buffered, MediaChain mediaHandlers = null)
        {
            Contract.Requires<ArgumentNullException>(reply != null);

            this.Message = reply;
            this.contentCopy = new Lazy<Task<ReadOnlySeekableStream>>(this.InitializeBufferedContent);

            var media = reply.Content?.Headers?.ContentType?.MediaType;
            var accepts = reply.RequestMessage?.Headers?.Accept;
            this.mediaHandler = mediaHandlers?.GetHandlerFor(media);
            this.MediaType = media ?? string.Empty;
            this.IsUnexpectedMediaType = accepts?.FirstOrDefault(h => h.MediaType.Equals(media, StringComparison.OrdinalIgnoreCase)) == null;
            this.StatusCode = reply.StatusCode;
            this.ReasonPhrase = reply.ReasonPhrase;
            this.Buffered = buffered;
        }

        public HttpResponseMessage Message { get; }

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

        public bool IsUnexpectedMediaType { get; }

        public string MediaType { get; }

        public bool Buffered { get; }

        public bool CanDeserialize => this.mediaHandler != null;

        public void Dispose()
        {
            this.Message.Dispose();
            GC.SuppressFinalize(this);
        }

        public TReply Deserialize<TReply>() => this.DeserializeAsync<TReply>().ConfigureAwait(false).GetAwaiter().GetResult();

        public dynamic GetContentAsXml(XmlConventions conventions = null) => this.GetContentAsXmlAsync(conventions).ConfigureAwait(false).GetAwaiter().GetResult();

        public XElement GetContentAsXElement() => this.GetContentAsXElementAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public XDocument GetContentAsXDocument() => this.GetContentAsXDocumentAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public XmlReader GetContentAsXmlReader() => this.GetContentAsXmlReaderAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public Dictionary<string, object> GetContentAsJsonMap() => this.GetContentAsJsonMapAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public dynamic GetContentAsJson(CasingConvention casing = CasingConvention.AsIs) => this.GetContentAsJsonAsync(casing).ConfigureAwait(false).GetAwaiter().GetResult();

        public Stream GetContentAsStream() => this.GetContentAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public string GetContentAsText() => this.GetContentAsTextAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
            return (await this.GetContentAsXElementAsync().ConfigureAwait(false))
                .ToDynamic(conventions ?? XmlConventions.Default);
        }

        public async Task<XElement> GetContentAsXElementAsync()
        {
            return XElement.Load(
                await this.GetContentAsStreamAsync().ConfigureAwait(false),
                LoadOptions.PreserveWhitespace);
        }

        public async Task<XDocument> GetContentAsXDocumentAsync()
        {
            return XDocument.Load(
                await this.GetContentAsStreamAsync().ConfigureAwait(false),
                LoadOptions.PreserveWhitespace);
        }

        public async Task<XmlReader> GetContentAsXmlReaderAsync()
        {
            return new XmlTextReader(await this.GetContentAsStreamAsync().ConfigureAwait(false));
        }

        public async Task<Dictionary<string, object>> GetContentAsJsonMapAsync() => new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(await this.GetContentAsTextAsync().ConfigureAwait(false));

        public async Task<dynamic> GetContentAsJsonAsync(CasingConvention casing = CasingConvention.AsIs) => (await this.GetContentAsJsonMapAsync().ConfigureAwait(false)).ToDynamic(casing);

        public async Task<Stream> GetContentAsStreamAsync()
        {
            var copy = await this.contentCopy.Value.ConfigureAwait(false);
            copy.Position = 0;
            return copy;
        }

        public async Task<string> GetContentAsTextAsync()
        {
            using (var reader = new StreamReader(await this.GetContentAsStreamAsync().ConfigureAwait(false)))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task<ReadOnlySeekableStream> InitializeBufferedContent()
        {
            return new ReadOnlySeekableStream(await ReadContent().ConfigureAwait(false));
        }

        private  async Task<Stream> ReadContent()
        {
            var source = await this.Message.Content.ReadAsStreamAsync().ConfigureAwait(false);
            if (!Buffered) return source;
            var s = new MemoryStream(DefaultBufferSize);
            source.CopyTo(s);
            return s;
        }
    }
}
