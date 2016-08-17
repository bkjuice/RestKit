using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace RestKit
{
    public sealed class Response<T> : IDisposable
    {
        private MediaHandler<T> mediaHandler;

        private Lazy<MemoryStream> contentCopy;

        private int defaultBufferSize;

        public Response(HttpResponseMessage reply, MediaHandler<T> mediaHandler, int defaultBufferSize = 4096)
        {
            Contract.Requires<ArgumentNullException>(reply != null);

            this.Message = reply;
            this.mediaHandler = mediaHandler;
            this.defaultBufferSize = defaultBufferSize;
            this.contentCopy = new Lazy<MemoryStream>(this.InitializeRawContent);

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

        public bool TryDeserialize(out T reply)
        {
            reply = default(T);
            return this.mediaHandler?.TryDeserialize(this.GetContentAsStream(), this.MediaType, out reply) == true;
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

        private MemoryStream InitializeRawContent()
        {
            var s = new MemoryStream(this.defaultBufferSize);
            this.Message.Content?.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult().CopyTo(s);
            s.Position = 0;
            return s;
        }
    }
}
