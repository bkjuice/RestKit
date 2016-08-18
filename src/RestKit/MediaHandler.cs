using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace RestKit
{
    public class MediaHandler : IMediaHandler
    {
        private Func<Stream, Type, object> handler;

        private string expectedMediaType;

        public MediaHandler(Func<Stream, Type, object> handler, string expectedMediaType)
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
            return this.Deserialize(content, target);
        }

        public bool CanDeserialize(string mediaType)
        {
            return mediaType?.Equals(this.expectedMediaType, StringComparison.OrdinalIgnoreCase) == true;
        }

        public object Deserialize(Stream content, Type target)
        {
            Contract.Requires<ArgumentNullException>(content != null);
            return this.handler(content, target);
        }
    }
}
