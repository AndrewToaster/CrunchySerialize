using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    /// <summary>
    /// Utility class used to encompass a type into <see cref="ISerializable"/> using factories
    /// </summary>
    /// <typeparam name="T">The generic type to encompass</typeparam>
    public sealed class SerializableWrap<T> : ISerializable
    {
        /// <summary>
        /// The factory used to serialize this wrapper
        /// </summary>
        public Action<ByteWriter> SerializeDelegate { get; }

        /// <summary>
        /// The factory used to deserialize this wrapper
        /// </summary>
        public Action<ByteBuffer> DeserializeDelegate { get; }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableWrap{T}"/>
        /// </summary>
        /// <param name="serialize">The factory used to serialize this wrapper</param>
        /// <param name="deserialize">The factory used to deserialize this wrapper</param>
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
