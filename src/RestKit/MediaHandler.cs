using System;
using System.IO;

namespace RestKit
{
    public class MediaHandler
    {
        private Func<Stream, object> handler;

        private string mediaType;

        public MediaHandler(Func<Stream, object> handler, string mediaType)
        {
            this.handler = handler;
            this.mediaType = mediaType;
        }

        public MediaHandler Next;

        public bool TryDeserialize(Stream content, string mediaType, out object reply)
        {
            if (mediaType.Equals(this.mediaType, StringComparison.OrdinalIgnoreCase))
            {
                reply = this.handler(content);
                return true;
            }

            reply = null;
            return this.Next?.TryDeserialize(content, mediaType, out reply) == true;
        }
    }
}
