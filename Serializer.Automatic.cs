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
            private const string NULL_STR = "\u034B\u0356\u035B";

            /// <summary>
            /// Serializes a object into a <see cref="ByteBuffer"/>
            /// </summary>
            /// <typeparam name="T">The generic type</typeparam>
            /// <param name="obj">The object to serialize</param>
            /// <param name="depth">The depth of search for serializable members</param>
            /// <returns><see cref="ByteBuffer"/> containing the serialized object</returns>
            public static ByteBuffer Serialize<T>(T obj, int depth = -1)
            {
                ByteWriter writer = new();
                _serializeIntoWriter(obj, writer, depth);
                return writer.GetBuffer();
            }

            /// <summary>
            /// Serializes a object into a <see cref="ByteWriter"/>
            /// </summary>
            /// <typeparam name="T">The generic type</typeparam>
            /// <param name="obj">The object to serialize</param>
            /// <param name="writer">The <see cref="ByteWriter"/> which to write into</param>
            /// <param name="depth">The depth of search for serializable members</param>
            public static void SerializeIntoWriter<T>(T obj, ByteWriter writer, int depth = -1) where T : ISerializable
            {
                SerializeIntoWriter(obj, writer, depth);
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
                return _deserializeFromBuffer(type, buffer, depth);
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
                return (T)_deserializeFromBuffer(typeof(T), buffer, depth);
            }

            private static void _serializeIntoWriter(object obj, ByteWriter writer, int depth)
            {
                if (depth == 0)
                    return;

                Type type = obj.GetType();
                foreach (var field in type.GetFields(ReflectionHelper.SerializableFlags).OrderBy(x => x.Name))
                {
                    if (field.IsInitOnly || field.HasIgnore() || field.IsBackingField())
                        continue;

                    object fieldValue = field.GetValue(obj);

                    if (fieldValue == null)
                    {
                        writer.WriteString(NULL_STR);
                    }
                    else if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        writer.WritePrimitive(fieldValue);
                    }
                    else if (ReflectionHelper.ImplementsISerializable(field.FieldType))
                    {
                        ((ISerializable)fieldValue).Serialize(writer);
                    }
                    else
                    {
                        _serializeIntoWriter(fieldValue, writer, depth - 1);
                    }
                }
                foreach (var prop in type.GetProperties(ReflectionHelper.SerializableFlags).OrderBy(x => x.Name))
                {
                    if (!prop.CanWrite || !prop.CanRead || prop.HasIgnore())
                        continue;

                    object propValue = prop.GetValue(obj);

                    if (propValue == null)
                    {
                        writer.WriteString(NULL_STR);
                    }
                    else if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        writer.WritePrimitive(propValue);
                    }
                    else if (ReflectionHelper.ImplementsISerializable(prop.PropertyType))
                    {
                        ((ISerializable)propValue).Serialize(writer);
                    }
                    else
                    {
                        _serializeIntoWriter(propValue, writer, depth - 1);
                    }
                }
            }

            private static object _deserializeFromBuffer(Type type, ByteBuffer buffer, int depth)
            {
                if (depth == 0)
                    return null;

                object obj = FormatterServices.GetUninitializedObject(type);

                ConstructorHint hint = ReflectionHelper.GetCtorHint(type);
                if (hint == ConstructorHint.BeforeAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                foreach (var field in type.GetFields(ReflectionHelper.SerializableFlags).OrderBy(x => x.Name))
                {
                    if (field.IsInitOnly || field.HasIgnore() || field.IsBackingField())
                        continue;

                    if (buffer.PeekString() == NULL_STR)
                    {
                        buffer.ReadString();
                        continue;
                    }

                    if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        field.SetValue(obj, buffer.ReadPrimitive(field.FieldType));
                    }
                    else if (ReflectionHelper.ImplementsISerializable(field.FieldType))
                    {
                        ((ISerializable)field.GetValue(obj)).Deserialize(buffer);
                    }
                    else
                    {
                        field.SetValue(obj, _deserializeFromBuffer(field.FieldType, buffer, depth - 1));
                    }
                }
                foreach (var prop in type.GetProperties(ReflectionHelper.SerializableFlags).OrderBy(x => x.Name))
                {
                    if (!prop.CanWrite || !prop.CanRead || prop.HasIgnore())
                        continue;

                    if (buffer.PeekString() == NULL_STR)
                    {
                        buffer.ReadString();
                        continue;
                    }

                    if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        prop.SetValue(obj, buffer.ReadPrimitive(prop.PropertyType));
                    }
                    else if (ReflectionHelper.ImplementsISerializable(prop.PropertyType))
                    {
                        ((ISerializable)prop.GetValue(obj)).Deserialize(buffer);
                    }
                    else
                    {
                        prop.SetValue(obj, _deserializeFromBuffer(prop.PropertyType, buffer, depth - 1));
                    }
                }

                if (hint == ConstructorHint.AfterAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                return obj;
            }
        }
    }
}
