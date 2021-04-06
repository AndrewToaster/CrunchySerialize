using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    public class SerializableWrap<T> : ISerializable
    {
        public Action<BufferWriter> SerializeDelegate { get; }
        public Action<ByteBuffer> DeserializeDelegate { get; }

        public SerializableWrap(Action<BufferWriter> serialize, Action<ByteBuffer> deserialize)
        {
            SerializeDelegate = serialize;
            DeserializeDelegate = deserialize;
        }

        public void Serialize(BufferWriter writer)
        {
            SerializeDelegate.Invoke(writer);
        }

        public void Deserialize(ByteBuffer data)
        {
            DeserializeDelegate.Invoke(data);
        }
    }
}
