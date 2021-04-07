using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize
{
    /// <summary>
    /// Interface used to present serializable types
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serializes this type and writes it into the <paramref name="writer"/>
        /// </summary>
        /// <param name="writer">The writer to which to write data to</param>
        void Serialize(ByteWriter writer);

        /// <summary>
        /// Deserializes binary data and constructs this type
        /// </summary>
        /// <param name="data">The <see cref="ByteBuffer"/> from which to read data</param>
        void Deserialize(ByteBuffer data);
    }
}
