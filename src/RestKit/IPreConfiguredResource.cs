using System;
using System.IO;

namespace RestKit
{
    public interface IPreConfiguredResource
    {
        Resource AsJson();

        Resource AsJson(string mediaType);

        Resource AsXml();

        Resource AsXml(string mediaType);

        Resource AsText();

        Resource AsText(string mediaType);

        Resource AsMedia(string mediaType, Action<object, Stream> serializer, Func<Stream, Type, object> deserializer);

        Resource AsRaw();
    }
}
