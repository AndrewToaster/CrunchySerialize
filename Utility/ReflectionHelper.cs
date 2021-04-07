using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CrunchySerialize.Attributes;

namespace CrunchySerialize.Utility
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, SerializableTypes> TypeDictionary = new()
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
            return TypeDictionary.ContainsKey(type) || type.IsEnum;
        }

        public static SerializableTypes GetSerializableType(Type type)
        {
            return type.IsEnum ? SerializableTypes.Enum : TypeDictionary[type];
        }

        public static IntegralTypes GetEnumType(Type type)
        {
            // Works as long as index stay same as in SerializableTypes
            return (IntegralTypes)TypeDictionary[type.GetEnumUnderlyingType()];
        }

        public static ConstructorHint GetCtorHint(Type type)
        {
            SerializationHintAttribute attribute = type.GetCustomAttribute<SerializationHintAttribute>();
            if (attribute != null)
            {
                return attribute.Hint;
            }
            else
            {
                return ConstructorHint.None;
            }
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>() != null;
        }

        public static ConstructorInfo GetDefaultCtor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static void InvokeDefaultConstructor(object obj)
        {
            obj.GetType().GetConstructor(Type.EmptyTypes)?.Invoke(obj, null);
        }

        public static bool IsISerializable(Type type)
        {
            return typeof(ISerializable).IsAssignableFrom(type);
        }
    }
}
