using System;
using System.Runtime.Serialization;
using CrunchySerialize.Attributes;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    public static partial class Serializator
    {
        public static class Automatic
        {
            public static ByteBuffer Serialize<T>(T obj, int depth = -1)
            {
                ByteWriter writer = new();
                SerializeIntoWriter(obj, writer, depth);
                return writer.GetByteBuffer();
            }

            public static object Deserialize(Type type, ByteBuffer buffer, int depth = -1)
            {
                return DeserializeFromBuffer(type, buffer, depth);
            }

            public static T Deserialize<T>(ByteBuffer buffer, int depth = -1)
            {
                return (T)DeserializeFromBuffer(typeof(T), buffer, depth);
            }

            private static void SerializeIntoWriter(object obj, ByteWriter writer, int depth)
            {
                if (depth == 0)
                    return;

                Type type = obj.GetType();
                foreach (var field in type.GetFields())
                {
                    object fieldValue = field.GetValue(obj);
                    if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        writer.WriteObject(fieldValue);
                    }
                    else if (ReflectionHelper.IsISerializable(field.FieldType))
                    {
                        ((ISerializable)fieldValue).Serialize(writer);
                    }
                    else
                    {
                        SerializeIntoWriter(fieldValue, writer, depth - 1);
                    }
                }
                foreach (var prop in type.GetProperties())
                {
                    object propValue = prop.GetValue(obj);
                    if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        writer.WriteObject(propValue);
                    }
                    else if (ReflectionHelper.IsISerializable(prop.PropertyType))
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

                foreach (var field in type.GetFields())
                {
                    if (field.FieldType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    if (ReflectionHelper.IsSerializableType(field.FieldType))
                    {
                        field.SetValue(obj, buffer.ReadObject(field.FieldType));
                    }
                    else if (ReflectionHelper.IsISerializable(field.FieldType))
                    {
                        ((ISerializable)field.GetValue(obj)).Deserialize(buffer);
                    }
                    else
                    {
                        field.SetValue(obj, DeserializeFromBuffer(field.FieldType, buffer, depth - 1));
                    }
                }
                foreach (var prop in type.GetProperties())
                {
                    if (prop.PropertyType.HasAttribute<IgnoreMemberAttribute>())
                        continue;

                    if (ReflectionHelper.IsSerializableType(prop.PropertyType))
                    {
                        prop.SetValue(obj, buffer.ReadObject(prop.PropertyType));
                    }
                    else if (ReflectionHelper.IsISerializable(prop.PropertyType))
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
