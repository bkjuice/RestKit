using System.IO;
using ProtoBuf;

namespace RestKit.Tests.Content
{
    public static class ProtoSerializer
    {
        public static void Serialize<T>(T target, Stream output)
        {
            Serializer.Serialize<T>(output, target);
            output.Flush();
        }

        public static T Deserialize<T>(Stream input)
        {
            return Serializer.Deserialize<T>(input);
        }
    }
}
