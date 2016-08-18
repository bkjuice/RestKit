using System;
using System.IO;

namespace RestKit
{
    public interface IRequestBody<out TRequest>
    {
        void SetSerializer(Action<TRequest, Stream> serializerAction);
    }
}
