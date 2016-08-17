using System.Collections.Generic;
using System.Linq;

namespace RestKit
{
    public class MediaChain
    {
        private List<IMediaHandler> handlers = new List<IMediaHandler>();

        public void AddHandler(IMediaHandler handler)
        {
            handlers.Add(handler);
        }

        public IMediaHandler GetHandlerFor(string mediaType)
        {
            return this.handlers.FirstOrDefault(h => h?.CanDeserialize(mediaType) == true);
        }
    }
}
