using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    public static class ReflectionHelper
    {
        public static readonly Dictionary<Type, SerializableType> SerializableStructs = new()
        {
            { typeof(int), SerializableType.Int },
            { typeof(uint), SerializableType.UInt },
            { typeof(long), SerializableType.Long },
            { typeof(ulong), SerializableType.ULong },
            { typeof(short), SerializableType.Short },
            { typeof(ushort), SerializableType.UShort },
            { typeof(byte), SerializableType.Byte },
            { typeof(char), SerializableType.Char },
            { typeof(bool), SerializableType.Bool },
            { typeof(string), SerializableType.String }
        };

        public static bool IsSerializableStruct(Type type)
        {
            return SerializableStructs.ContainsKey(type);
        }

        public static SerializableType GetSerializableType(Type type)
        {
            return SerializableStructs[type];
        }

        public static bool IsISerializable(Type type)
        {
            return typeof(ISerializable).IsAssignableFrom(type);
        }
    }
}
