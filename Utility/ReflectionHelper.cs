using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CrunchySerialize.Attributes;

namespace CrunchySerialize.Utility
{
    /// <summary>
    /// Helper class used for helping with reflection
    /// </summary>
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

        /// <summary>
        /// Check whether or not <paramref name="type"/> is a serializable type
        /// </summary>
        /// <param name="type">The type to check</param>
        public static bool IsSerializableType(Type type)
        {
            return TypeDictionary.ContainsKey(type) || type.IsEnum;
        }

        /// <summary>
        /// Gets <see cref="SerializableTypes"/> value of the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check against</param>
        public static SerializableTypes GetSerializableType(Type type)
        {
            return type.IsEnum ? SerializableTypes.Enum : TypeDictionary[type];
        }

        /// <summary>
        /// Gets <see cref="IntegralTypes"/> value of the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check against</param>
        public static IntegralTypes GetEnumType(Type type)
        {
            // Works as long as index stay same as in SerializableTypes
            return (IntegralTypes)TypeDictionary[type.GetEnumUnderlyingType()];
        }

        /// <summary>
        /// Gets the <see cref="ConstructorHint"/> value of the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to check against</param>
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

        /// <summary>
        /// Checks whether or not the <paramref name="type"/> has a generic attribute
        /// </summary>
        /// <param name="type">The type to check against</param>
        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>() != null;
        }

        /// <summary>
        /// Gets the default (empty) constructor of the <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type to get its constructor</param>
        /// <returns>The constructor (can be null)</returns>
        public static ConstructorInfo GetDefaultCtor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        /// <summary>
        /// Invokes a default (empty) constructor of the <paramref name="obj"/> if it is found
        /// </summary>
        /// <param name="obj">The object to execute it's constructor</param>
        public static void InvokeDefaultConstructor(object obj)
        {
            obj.GetType().GetConstructor(Type.EmptyTypes)?.Invoke(obj, null);
        }

        /// <summary>
        /// Checks whether or not the <paramref name="type"/> implements <see cref="ISerializable"/> interface
        /// </summary>
        /// <param name="type">The type to check against</param>
        public static bool IsISerializable(Type type)
        {
            return typeof(ISerializable).IsAssignableFrom(type);
        }
    }
}
