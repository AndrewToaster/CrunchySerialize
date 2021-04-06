using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, SerializableTypes> SerializableStructs = new()
        {
            { typeof(int), SerializableTypes.Int },
            { typeof(uint), SerializableTypes.UInt },
            { typeof(long), SerializableTypes.Long },
            { typeof(ulong), SerializableTypes.ULong },
            { typeof(short), SerializableTypes.Short },
            { typeof(ushort), SerializableTypes.UShort },
            { typeof(byte), SerializableTypes.Byte },
            { typeof(char), SerializableTypes.Char },
            { typeof(bool), SerializableTypes.Bool },
            { typeof(string), SerializableTypes.String },
            { typeof(Enum), SerializableTypes.Enum }
        };

        public static bool IsSerializableType(Type type)
        {
            return SerializableStructs.ContainsKey(type);
        }

        public static SerializableTypes GetSerializableType(Type type)
        {
            return SerializableStructs[type];
        }

        public static IntegralTypes GetEnumType(Type type)
        {
            // Works as long as index stay same as in SerializableTypes
            return (IntegralTypes)SerializableStructs[type.GetEnumUnderlyingType()];
        }

        public static bool IsISerializable(Type type)
        {
            return typeof(ISerializable).IsAssignableFrom(type);
        }
    }
}
