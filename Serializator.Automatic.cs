using System;
using System.Runtime.Serialization;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    public static partial class Serializator
    {
        public static class Automatic
        {
            public static ByteBuffer Serialize(object obj)
            {
                Type type = obj.GetType();
                ByteWriter writer = new();

                ConstructorHint hint = ReflectionHelper.GetCtorHint(type);
                if (hint == ConstructorHint.BeforeAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                foreach (var field in type.GetFields())
                {
                    if (ReflectionHelper.IsSerializableType(type))
                    {
                        writer.WriteObject(field.GetValue(obj));
                    }
                    else if (ReflectionHelper.IsISerializable(type))
                    {
                        ((ISerializable)field.GetValue(obj)).Serialize(writer);
                    }
                }
                foreach (var prop in type.GetProperties())
                {
                    if (ReflectionHelper.IsSerializableType(type))
                    {
                        writer.WriteObject(prop.GetValue(obj));
                    }
                    else if (ReflectionHelper.IsISerializable(type))
                    {
                        ((ISerializable)prop.GetValue(obj)).Serialize(writer);
                    }
                }

                if (hint == ConstructorHint.AfterAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                return writer.GetByteBuffer();
            }

            public static ByteBuffer Serialize<T>(T obj)
            {
                return Serialize(obj);
            }

            public static object Deserialize(Type type, ByteBuffer buffer)
            {
                object obj = FormatterServices.GetUninitializedObject(type);

                ConstructorHint hint = ReflectionHelper.GetCtorHint(type);
                if (hint == ConstructorHint.BeforeAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                foreach (var field in type.GetFields())
                {
                    if (ReflectionHelper.IsSerializableType(type))
                    {
                        field.SetValue(obj, buffer.ReadObject(field.FieldType));
                    }
                    else if (ReflectionHelper.IsISerializable(type))
                    {
                        ((ISerializable)field.GetValue(obj)).Deserialize(buffer);
                    }
                }
                foreach (var prop in type.GetProperties())
                {
                    if (ReflectionHelper.IsSerializableType(type))
                    {
                        prop.SetValue(obj, buffer.ReadObject(prop.PropertyType));
                    }
                    else if (ReflectionHelper.IsISerializable(type))
                    {
                        ((ISerializable)prop.GetValue(obj)).Deserialize(buffer);
                    }
                }

                if (hint == ConstructorHint.AfterAssignment)
                    ReflectionHelper.InvokeDefaultConstructor(obj);

                return obj;
            }

            public static T Deserialize<T>(ByteBuffer buffer)
            {
                return (T)Deserialize(typeof(T), buffer);
            }
        }
    }
}
