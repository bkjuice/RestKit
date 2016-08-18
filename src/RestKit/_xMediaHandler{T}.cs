using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace RestKit
{
    public class MediaHandler<T> : IMediaHandler
    {
        private Func<Stream, T> handler;

        private string expectedMediaType;

        public MediaHandler(Func<Stream, T> handler, string expectedMediaType)
        {
            Contract.Requires<ArgumentNullException>(handler != null);
            Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(expectedMediaType) == false);

            this.handler = handler;
            this.expectedMediaType = expectedMediaType;
        }

        bool IMediaHandler.CanDeserialize(string mediaType)
        {
            return this.CanDeserialize(mediaType);
        }

        object IMediaHandler.Deserialize(Stream content, Type target)
        {
            return this.Deserialize(content);
        }

        public bool CanDeserialize(string mediaType)
        {
            return mediaType?.Equals(this.expectedMediaType, StringComparison.OrdinalIgnoreCase) == true;
        }

        public T Deserialize(Stream content)
        {
            Contract.Requires<ArgumentNullException>(content != null);
            return this.handler(content);
        }
    }
}
