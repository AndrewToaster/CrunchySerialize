using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    public class SerializableWrap<T> : ISerializable
    {
        public Action<ByteWriter> SerializeDelegate { get; }
        public Action<ByteBuffer> DeserializeDelegate { get; }

        public SerializableWrap(Action<ByteWriter> serialize, Action<ByteBuffer> deserialize)
        {
            SerializeDelegate = serialize;
            DeserializeDelegate = deserialize;
        }

        public void Serialize(ByteWriter writer)
        {
            SerializeDelegate.Invoke(writer);
        }

        public void Deserialize(ByteBuffer data)
        {
            DeserializeDelegate.Invoke(data);
        }
    }
}
