using System;
using System.IO;

namespace RestKit
{
    public class MediaHandler
    {
        private readonly Func<Stream, Type, object> handler;

        private readonly string expectedMediaType;

        public MediaHandler(Func<Stream, Type, object> handler, string expectedMediaType)
        {
            handler.DisallowNull(nameof(handler));
            expectedMediaType.DisallowNullOrEmpty(nameof(expectedMediaType));

            this.handler = handler;
            this.expectedMediaType = expectedMediaType;
        }

        public bool CanDeserialize(string mediaType)
        {
            return mediaType?.Equals(this.expectedMediaType, StringComparison.OrdinalIgnoreCase) == true;
        }

        public object Deserialize(Stream content, Type target)
        {
            content.DisallowNull(nameof(content));
            return this.handler(content, target);
        }
    }
}
