﻿using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    public class ByteWriter
    {
        private readonly ArrayBufferWriter<byte> _buffer;

        public ByteWriter()
        {
            _buffer = new ArrayBufferWriter<byte>();
        }

        public ByteWriter(Span<byte> data)
        {
            _buffer = new ArrayBufferWriter<byte>(data.Length);
            _buffer.Write(data);
        }

        public void WriteSpan(Span<byte> data)
        {
            _buffer.Write(data);
        }

        public void WriteByte(byte val)
        {
            Span<byte> data = stackalloc byte[1];
            data[0] = val;
            _buffer.Write(data);
        }

        public void WriteInt(int val)
        {
            Span<byte> data = stackalloc byte[sizeof(int)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteUInt(uint val)
        {
            Span<byte> data = stackalloc byte[sizeof(uint)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteLong(long val)
        {
            Span<byte> data = stackalloc byte[sizeof(long)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteULong(ulong val)
        {
            Span<byte> data = stackalloc byte[sizeof(ulong)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteShort(short val)
        {
            Span<byte> data = stackalloc byte[sizeof(short)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteUShort(ushort val)
        {
            Span<byte> data = stackalloc byte[sizeof(ushort)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteBool(bool val)
        {
            Span<byte> data = stackalloc byte[sizeof(bool)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteChar(char val)
        {
            Span<byte> data = stackalloc byte[sizeof(char)];
            BitConverter.TryWriteBytes(data, val);
            WriteSpan(data);
        }

        public void WriteString(string val)
        {
            ReadOnlySpan<char> span = val.AsSpan();

            if (span.IsEmpty)
                return;

            WriteInt(span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                WriteChar(span[i]);
            }
        }

        public void WriteObject(object obj)
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
                    switch (ReflectionHelper.GetEnumType(type))
                    {
                        case IntegralTypes.Int:
                            WriteInt((int)obj);
                            break;

                        case IntegralTypes.UInt:
                            WriteUInt((uint)obj);
                            break;

                        case IntegralTypes.Long:
                            WriteLong((long)obj);
                            break;

                        case IntegralTypes.ULong:
                            WriteULong((ulong)obj);
                            break;

                        case IntegralTypes.Short:
                            WriteShort((short)obj);
                            break;

                        case IntegralTypes.UShort:
                            WriteUShort((ushort)obj);
                            break;

                        case IntegralTypes.Byte:
                            WriteByte((byte)obj);
                            break;
                    }
                    break;
            }
        }

        public ReadOnlySpan<byte> GetByteSpan()
        {
            return _buffer.WrittenSpan;
        }

        public ByteBuffer GetByteBuffer()
        {
            return new(_buffer.WrittenSpan);
        }
    }
}