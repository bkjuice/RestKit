using ProtoBuf;

namespace RestKit.Tests.Content
{
    [ProtoContract]
    public class SimpleItem
    {
        [ProtoMember(1)]
        public string Value { get; set; }
    }
}
