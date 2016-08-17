using System;
using System.IO;

namespace RestKit
{
    public interface IMediaConfiguration<out TRequest>
    {
        void SetSerializer(Action<TRequest, Stream> serializerAction);
    }
}
