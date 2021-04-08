using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
        /// Section of <see cref="Serializer"/> that uses reflection to serialize primitives and <see cref="ISerializable"/> instances
        /// </summary>
        public static class Automatic
        {
            /// <summary>
            /// Serializes a object into <see cref="ByteBuffer"/>
            /// </summary>
            /// <typeparam name="T">The generic type</typeparam>
            /// <param name="obj">The object to serialize</param>
            /// <param name="depth">The depth of search for serializable members</param>
            /// <returns><see cref="ByteBuffer"/> containing the serialized object</returns>
            public static ByteBuffer Serialize<T>(T obj, int depth = -1)
            {
                ByteWriter writer = new();
                SerializeIntoWriter(obj, writer, depth);
                return writer.GetByteBuffer();
            }

            /// <summary>
            /// Deserializes a <see cref="ByteBuffer"/> into a object
            /// </summary>
            /// <remarks>
            /// <see cref="ByteBuffer"/> <paramref name="buffer"/> is not disposed of!
            /// </remarks>
            /// <param name="type">The type of object to deserialize into</param>
            /// <param name="buffer">The <see cref="ByteBuffer"/> to read data from</param>
            /// <param name="depth">The depth of search for deserializable members</param>
            /// <returns>Serialized object</returns>
            public static object Deserialize(Type type, ByteBuffer buffer, int depth = -1)
            {
                return DeserializeFromBuffer(type, buffer, depth);
            }

            /// <summary>
            /// Deserializes a <see cref="ByteBuffer"/> into a generic object
            /// </summary>
            /// <remarks>
            /// <see cref="ByteBuffer"/> <paramref name="buffer"/> is not disposed of!
            /// </remarks>
            /// <typeparam name="T">The generic type</typeparam>
            /// <param name="buffer">The <see cref="ByteBuffer"/> to read data from</param>
            /// <param name="depth">The depth of search for deserializable members</param>
            /// <returns>Serialized object <typeparamref name="T"/></returns>
            public static T Deserialize<T>(ByteBuffer buffer, int depth = -1)
            {
                return (T)DeserializeFromBuffer(typeof(T), buffer, depth);
            }

            private static void SerializeIntoWriter(object obj, ByteWriter writer, int depth)
            {
                if (depth == 0)
                    return;

                Type type = obj.GetType();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (field.FieldType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    object fieldValue = field.GetValue(obj);
                    if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        writer.WriteObject(fieldValue);
                    }
                    else if (ReflectionHelper.ImplementsISerializable(field.FieldType))
                    {
                        ((ISerializable)fieldValue).Serialize(writer);
                    }
                    else
                    {
                        SerializeIntoWriter(fieldValue, writer, depth - 1);
                    }
                }
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.PropertyType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    object propValue = prop.GetValue(obj);
                    if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        writer.WriteObject(propValue);
                    }
                    else if (ReflectionHelper.ImplementsISerializable(prop.PropertyType))
                    {
                        ((ISerializable)propValue).Serialize(writer);
                    }
                    else
                    {
                        SerializeIntoWriter(propValue, writer, depth - 1);
                    }
                }
            }

            private static object DeserializeFromBuffer(Type type, ByteBuffer buffer, int depth)
            {
                if (depth == 0)
                    return null;

                object obj = FormatterServices.GetUninitializedObject(type);

                ConstructorHint hint = ReflectionHelper.GetCtorHint(type);
                if (hint == ConstructorHint.BeforeAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (field.FieldType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        field.SetValue(obj, buffer.ReadObject(field.FieldType));
                    }
                    else if (ReflectionHelper.ImplementsISerializable(field.FieldType))
                    {
                        ((ISerializable)field.GetValue(obj)).Deserialize(buffer);
                    }
                    else
                    {
                        field.SetValue(obj, DeserializeFromBuffer(field.FieldType, buffer, depth - 1));
                    }
                }
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(x => x.Name))
                {
                    if (prop.PropertyType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        prop.SetValue(obj, buffer.ReadObject(prop.PropertyType));
                    }
                    else if (ReflectionHelper.ImplementsISerializable(prop.PropertyType))
                    {
                        ((ISerializable)prop.GetValue(obj)).Deserialize(buffer);
                    }
                    else
                    {
                        prop.SetValue(obj, DeserializeFromBuffer(prop.PropertyType, buffer, depth - 1));
                    }
                }

                if (hint == ConstructorHint.AfterAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                return obj;
            }
        }
    }
}
