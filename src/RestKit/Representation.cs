using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace RestKit
{
    public sealed class Representation : IDisposable
    {
        private MediaHandler mediaHandler;

        private Lazy<ReadOnlySeekableStream> contentCopy;

        private int defaultBufferSize;

        public Representation(HttpResponseMessage reply, MediaHandler mediaHandler, int defaultBufferSize = 4096)
        {
            Contract.Requires<ArgumentNullException>(reply != null);

            this.Message = reply;
            this.mediaHandler = mediaHandler;
            this.defaultBufferSize = defaultBufferSize;
            this.contentCopy = new Lazy<ReadOnlySeekableStream>(this.InitializeRawContent);

            var media = reply.Content?.Headers?.ContentType?.MediaType;
            var accepts = reply.RequestMessage?.Headers?.Accept;
            this.MediaType = media;
            this.MediaIsExpected = accepts?.FirstOrDefault(h => h.MediaType.Equals(media, StringComparison.OrdinalIgnoreCase)) != null;
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

        public bool MediaIsExpected { get; private set; }

        public string MediaType { get; private set; }

        public bool TryDeserialize<TReply>(out TReply reply)
        {
            reply = default(TReply);
            object output = null;
            if( this.mediaHandler?.TryDeserialize(this.GetContentAsStream(), this.MediaType, out output) == true)
            {
                reply = (TReply)output;
                return true;
            }

            return false;
        }

        public XElement GetContentAsXElement()
        {
            return XElement.Load(this.GetContentAsStream(), LoadOptions.PreserveWhitespace);
        }

        public XDocument GetContentAsXDocument()
        {
            return XDocument.Load(this.GetContentAsStream(), LoadOptions.PreserveWhitespace);
        }

        public dynamic GetContentAsXml()
        {
            return this.GetContentAsXElement().ToDynamic();
        }

        public XmlReader GetContentAsXmlReader()
        {
            return new XmlTextReader(this.GetContentAsStream());
        }

        public Dictionary<string, object> GetContentAsJsonMap()
        {
            return new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(this.GetContentAsString());
        }

        public dynamic GetContentAsJson()
        {
            return this.GetContentAsJsonMap().ToDynamic();
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

        public void Dispose()
        {
            this.Message.Dispose();
            GC.SuppressFinalize(this);
        }

        private ReadOnlySeekableStream InitializeRawContent()
        {
            var s = new MemoryStream(this.defaultBufferSize);
            this.Message.Content?.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult().CopyTo(s);
            return new ReadOnlySeekableStream(s);
        }
    }
}
