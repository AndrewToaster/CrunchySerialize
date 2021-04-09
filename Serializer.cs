using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CrunchySerialize.Attributes;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    /// <summary>
    /// Class containing functionality for Serializing (into bytes) and Deserializing (from bytes)
    /// </summary>
    public static partial class Serializer
    {
        /// <summary>
        /// Serializes a object into a <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns><see cref="ByteBuffer"/> containing the serialized object</returns>
        public static ByteBuffer Serialize<T>(T obj) where T : ISerializable
        {
            ByteWriter writer = new();
            obj.Serialize(writer);
            return writer.GetBuffer();
        }

        /// <summary>
        /// Serializes a object into a <see cref="ByteWriter"/>
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writer">The <see cref="ByteWriter"/> which to write into</param>
        public static void SerializeIntoWriter<T>(T obj, ByteWriter writer) where T : ISerializable
        {
            obj.Serialize(writer);
        }

        /// <summary>
        /// Deserializes a <see cref="ByteBuffer"/> into a generic object
        /// </summary>
        /// <see cref="ByteBuffer"/> <paramref name="buffer"/> is not disposed of!
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="buffer">The <see cref="ByteBuffer"/> to read data from</param>
        /// <returns>Serialized object <typeparamref name="T"/></returns>
        public static T Deserialize<T>(ByteBuffer buffer) where T : ISerializable
        {
            Type type = typeof(T);
            T obj = (T)FormatterServices.GetUninitializedObject(type);

            switch (ReflectionHelper.GetCtorHint(type))
            {
                case ConstructorHint.AfterAssignment:
                    {
                        obj.Deserialize(buffer);
                        ReflectionHelper.InvokeDefaultConstructor(obj);
                    }
                    break;

                case ConstructorHint.BeforeAssignment:
                    {
                        ReflectionHelper.InvokeDefaultConstructor(obj);
                        obj.Deserialize(buffer);
                    }
                    break;

                case ConstructorHint.None:
                    obj.Deserialize(buffer);
                    break;
            }

            return obj;
        }

        /// <summary>
        /// Deserializes a <see cref="ByteBuffer"/> into a object
        /// </summary>
        /// <remarks>
        /// <see cref="ByteBuffer"/> <paramref name="buffer"/> is not disposed of!
        /// </remarks>
        /// <param name="type">The type of object to deserialize into</param>
        /// <param name="buffer">The <see cref="ByteBuffer"/> to read data from</param>
        /// <returns>Serialized object</returns>
        public static object Deserialize(Type type, ByteBuffer buffer)
        {
            if (!typeof(ISerializable).IsAssignableFrom(type))
            {
                throw new ArgumentException($"The specified type does not inherit from {nameof(ISerializable)}", nameof(type));
            }

            ISerializable obj = (ISerializable)FormatterServices.GetUninitializedObject(type);

            switch (ReflectionHelper.GetCtorHint(type))
            {
                case ConstructorHint.AfterAssignment:
                    {
                        obj.Deserialize(buffer);
                        ReflectionHelper.InvokeDefaultConstructor(obj);
                    }
                    break;

                case ConstructorHint.BeforeAssignment:
                    {
                        ReflectionHelper.InvokeDefaultConstructor(obj);
                        obj.Deserialize(buffer);
                    }
                    break;

                case ConstructorHint.None:
                    obj.Deserialize(buffer);
                    break;
            }

            return obj;
        }
    }
}
