using System;
using System.IO;

namespace RestKit
{
    public interface IMediaHandler
    {
        bool CanDeserialize(string mediaType);

        object Deserialize(Stream content);
    }
}
