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
    public static partial class Serializator
    {
        public static ByteBuffer Serialize<T>(T obj) where T : ISerializable
        {
            ByteWriter writer = new();
            obj.Serialize(writer);
            return writer.GetByteBuffer();
        }

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
