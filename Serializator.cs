using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

        /*public static ByteBuffer Serialize(ISerializable obj)
        {
            BufferWriter writer = new();
            obj.Serialize(writer);
            return new ByteBuffer(writer.GetByteSpan());
        }*/

        public static T Deserialize<T>(ByteBuffer buffer) where T : ISerializable
        {
            Type type = typeof(T);
            T obj = (T)FormatterServices.GetUninitializedObject(type);
            SerializationHintAttribute attr = type.GetCustomAttribute<SerializationHintAttribute>();

            if (attr != null && attr.Hint != ConstructorHint.Ignore)
            {
                switch (attr.Hint)
                {
                    case ConstructorHint.DefaultPostInit:
                        {
                            obj.Deserialize(buffer);
                            type.GetConstructor(Type.EmptyTypes).Invoke(obj, null);
                        }
                        break;

                    case ConstructorHint.DefaultPreInit:
                        {
                            type.GetConstructor(Type.EmptyTypes).Invoke(obj, null);
                            obj.Deserialize(buffer);
                        }
                        break;
                }
            }
            else
            {
                obj.Deserialize(buffer);
            }

            return obj;
        }

        public static object Deserialize(Type type, ByteBuffer buffer)
        {
            ISerializable obj = (ISerializable)FormatterServices.GetUninitializedObject(type);
            SerializationHintAttribute attr = type.GetCustomAttribute<SerializationHintAttribute>();

            if (attr != null && attr.Hint != ConstructorHint.Ignore)
            {
                switch (attr.Hint)
                {
                    case ConstructorHint.DefaultPostInit:
                        {
                            obj.Deserialize(buffer);
                            type.GetConstructor(Type.EmptyTypes).Invoke(obj, null);
                        }
                        break;

                    case ConstructorHint.DefaultPreInit:
                        {
                            type.GetConstructor(Type.EmptyTypes).Invoke(obj, null);
                            obj.Deserialize(buffer);
                        }
                        break;
                }
            }
            else
            {
                obj.Deserialize(buffer);
            }

            return obj;
        }
    }
}
