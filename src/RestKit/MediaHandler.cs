using System;
using System.IO;

namespace RestKit
{
    public class MediaHandler<TReply>
    {
        private Func<Stream, TReply> handler;

        private string mediaType;

        public MediaHandler(Func<Stream, TReply> handler, string mediaType)
        {
            this.handler = handler;
            this.mediaType = mediaType;
        }

        public MediaHandler<TReply> Next;

        public bool TryDeserialize(Stream content, string mediaType, out TReply reply)
        {
            if (mediaType.Equals(this.mediaType, StringComparison.OrdinalIgnoreCase))
            {
                reply = this.handler(content);
                return true;
            }

            reply = default(TReply);
            return this.Next?.TryDeserialize(content, mediaType, out reply) == true;
        }
    }
}
