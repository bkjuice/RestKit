using System.Collections.Generic;
using System.Linq;

namespace RestKit
{
    public class MediaChain
    {
        private List<MediaHandler> handlers = new List<MediaHandler>();

        public void AddHandler(MediaHandler handler)
        {
            handlers.Add(handler);
        }

        public MediaHandler GetHandlerFor(string mediaType)
        {
            return this.handlers.FirstOrDefault(h => h?.CanDeserialize(mediaType) == true);
        }
    }
}
