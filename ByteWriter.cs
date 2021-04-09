using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    /// <summary>
    /// Class for writing binary data into a buffer
    /// </summary>
    public sealed class ByteWriter
    {
        private readonly ArrayBufferWriter<byte> _buffer;

        /// <summary>
        /// Creates a new instance of <see cref="ByteBuffer"/>
        /// </summary>
        public ByteWriter()
        {
            _buffer = new ArrayBufferWriter<byte>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ByteBuffer"/> with <paramref name="data"/> written into it
        /// </summary>
        public ByteWriter(Span<byte> data)
        {
            _buffer = new ArrayBufferWriter<byte>(data.Length);
            _buffer.Write(data);
        }

        #region Single Values

        /// <summary>
        /// Writes a binary <see cref="ReadOnlySpan{T}"/> into the internal buffer
        /// </summary>
        /// <param name="data"></param>
        public void WriteSpan(ReadOnlySpan<byte> data)
        {
            _buffer.Write(data);
        }

        /// <summary>
        /// Writes a <see cref="byte"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteByte(byte val)
        {
            Span<byte> data = stackalloc byte[1];
            data[0] = val;
            _buffer.Write(data);
        }

        /// <summary>
        /// Writes a <see cref="int"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteInt(int val)
        {
            Span<byte> data = stackalloc byte[sizeof(int)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="uint"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteUInt(uint val)
        {
            Span<byte> data = stackalloc byte[sizeof(uint)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="long"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteLong(long val)
        {
            Span<byte> data = stackalloc byte[sizeof(long)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="ulong"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteULong(ulong val)
        {
            Span<byte> data = stackalloc byte[sizeof(ulong)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="short"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteShort(short val)
        {
            Span<byte> data = stackalloc byte[sizeof(short)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="ushort"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteUShort(ushort val)
        {
            Span<byte> data = stackalloc byte[sizeof(ushort)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="bool"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteBool(bool val)
        {
            Span<byte> data = stackalloc byte[sizeof(bool)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="char"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteChar(char val)
        {
            Span<byte> data = stackalloc byte[sizeof(char)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        /// <summary>
        /// Writes a <see cref="string"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteString(string val)
        {
            byte[] data = Encoding.Default.GetBytes(val);
            WriteInt(data.Length);
            WriteArray(data);
        }

        /// <summary>
        /// Writes a <see cref="object"/> into the internal buffer depending on the <see cref="object"/>'s type
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to write into the internal buffer</param>
        public void WritePrimitive(object obj)
        {
            Type type = obj.GetType();
            switch (ReflectionHelper.GetSerializableType(type))
            {
                case SerializableTypes.Int:
                    WriteInt((int)obj);
                    break;

                case SerializableTypes.UInt:
                    WriteUInt((uint)obj);
                    break;

                case SerializableTypes.Long:
                    WriteLong((long)obj);
                    break;

                case SerializableTypes.ULong:
                    WriteULong((ulong)obj);
                    break;

                case SerializableTypes.Short:
                    WriteShort((short)obj);
                    break;

                case SerializableTypes.UShort:
                    WriteUShort((ushort)obj);
                    break;

                case SerializableTypes.Byte:
                    WriteByte((byte)obj);
                    break;

                case SerializableTypes.Char:
                    WriteChar((char)obj);
                    break;

                case SerializableTypes.Bool:
                    WriteBool((bool)obj);
                    break;

                case SerializableTypes.String:
                    WriteString((string)obj);
                    break;

                case SerializableTypes.Enum:
                    WriteEnum((Enum)obj);
                    break;
            }
        }

        /// <summary>
        /// Writes a <see cref="Enum"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        public void WriteEnum(Enum val)
        {
            switch (ReflectionHelper.GetEnumType(val.GetType().GetEnumUnderlyingType()))
            {
                case IntegralTypes.Int:
                    WriteInt((int)(object)val);
                    break;

                case IntegralTypes.UInt:
                    WriteUInt((uint)(object)val);
                    break;

                case IntegralTypes.Long:
                    WriteLong((long)(object)val);
                    break;

                case IntegralTypes.ULong:
                    WriteULong((ulong)(object)val);
                    break;

                case IntegralTypes.Short:
                    WriteShort((short)(object)val);
                    break;

                case IntegralTypes.UShort:
                    WriteUShort((ushort)(object)val);
                    break;

                case IntegralTypes.Byte:
                    WriteByte((byte)(object)val);
                    break;
            }
        }

        /// <summary>
        /// Writes a generic <see cref="Enum"/> <typeparamref name="T"/> into the internal buffer
        /// </summary>
        /// <param name="val">The value to write into the internal buffer</param>
        /// <typeparam name="T">The generic enum</typeparam>
        public void WriteEnum<T>(T val) where T : Enum
        {
            switch (ReflectionHelper.GetEnumType(typeof(T).GetEnumUnderlyingType()))
            {
                case IntegralTypes.Int:
                    WriteInt((int)(object)val);
                    break;

                case IntegralTypes.UInt:
                    WriteUInt((uint)(object)val);
                    break;

                case IntegralTypes.Long:
                    WriteLong((long)(object)val);
                    break;

                case IntegralTypes.ULong:
                    WriteULong((ulong)(object)val);
                    break;

                case IntegralTypes.Short:
                    WriteShort((short)(object)val);
                    break;

                case IntegralTypes.UShort:
                    WriteUShort((ushort)(object)val);
                    break;

                case IntegralTypes.Byte:
                    WriteByte((byte)(object)val);
                    break;
            }
        }

        #endregion Single Values

        #region Arrays

        /// <summary>
        /// Writes a <see cref="byte"/> array into the internal buffer
        /// </summary>
        /// <param name="data"></param>
        public void WriteArray(byte[] data)
        {
            _buffer.Write(data);
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(int[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteInt(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(uint[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteUInt(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(long[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteLong(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(ulong[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteULong(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(short[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteShort(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(ushort[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteUShort(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(char[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteChar(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(bool[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteBool(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(string[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteString(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray(Enum[] array)
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteEnum(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="array">The primitive array</param>
        public void WriteArray<TEnum>(TEnum[] array) where TEnum : Enum
        {
            WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                WriteEnum(array[i]);
            }
        }

        #endregion Arrays

        /// <summary>
        /// Gets the internal buffer of this <see cref="ByteWriter"/>
        /// </summary>
        /// <returns>The internal buffer as a binary <see cref="ReadOnlySpan{T}"/></returns>
        public ReadOnlySpan<byte> GetSpan()
        {
            return _buffer.WrittenSpan;
        }

        /// <summary>
        /// Gets the internal buffer of this <see cref="ByteWriter"/>
        /// </summary>
        /// <returns>The internal buffer as a binary <see cref="ReadOnlyMemory{T}"/></returns>
        public ReadOnlyMemory<byte> GetMemory()
        {
            return _buffer.WrittenMemory;
        }

        /// <summary>
        /// Gets the internal buffer of this <see cref="ByteWriter"/>
        /// </summary>
        /// <returns>The internal buffer as a <see cref="byte"/> array</returns>
        public byte[] GetArray()
        {
            return _buffer.WrittenMemory.ToArray();
        }

        /// <summary>
        /// Gets the internal buffer of this <see cref="ByteWriter"/>
        /// </summary>
        /// <returns>The internal buffer as <see cref="ByteBuffer"/></returns>
        public ByteBuffer GetBuffer()
        {
            return new(_buffer.WrittenSpan);
        }
    }
}
