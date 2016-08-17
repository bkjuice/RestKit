using System.Runtime.Serialization;
using ProtoBuf;

namespace RestKit.Tests.Content
{
    [DataContract]
    [ProtoContract]
    public class SimpleItem
    {
        [DataMember(Name ="value")]
        [ProtoMember(1)]
        public string Value { get; set; }
    }
}
