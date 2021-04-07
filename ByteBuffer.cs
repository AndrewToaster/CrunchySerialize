using System;
using System.Buffers;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    public class ByteBuffer : IDisposable
    {
        private readonly IMemoryOwner<byte> _buffer;
        private int _position;

        public ByteBuffer()
        {
            _buffer = MemoryPool<byte>.Shared.Rent();
        }

        public ByteBuffer(ReadOnlySpan<byte> data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.ToArray().CopyTo(_buffer.Memory);
        }

        #region Read And Advance

        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadData(sizeof(int)));
        }

        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(ReadData(sizeof(uint)));
        }

        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadData(sizeof(long)));
        }

        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(ReadData(sizeof(ulong)));
        }

        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadData(sizeof(short)));
        }

        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(ReadData(sizeof(ushort)));
        }

        public byte ReadByte()
        {
            byte data = _buffer.Memory.Span[_position];
            Advance(1);
            return data;
        }

        public char ReadChar()
        {
            return BitConverter.ToChar(ReadData(sizeof(char)));
        }

        public bool ReadBool()
        {
            return BitConverter.ToBoolean(ReadData(sizeof(bool)));
        }

        public string ReadString()
        {
            int len = ReadInt();
            Span<char> span = stackalloc char[len];

            for (int i = 0; i < len; i++)
            {
                span[i] = ReadChar();
            }

            return new string(span);
        }

        public T ReadEnum<T>() where T : Enum
        {
            return ReflectionHelper.GetEnumType(typeof(T)) switch
            {
                IntegralTypes.Int => (T)(object)ReadInt(),
                IntegralTypes.UInt => (T)(object)ReadUInt(),
                IntegralTypes.Long => (T)(object)ReadLong(),
                IntegralTypes.ULong => (T)(object)ReadULong(),
                IntegralTypes.Short => (T)(object)ReadShort(),
                IntegralTypes.UShort => (T)(object)ReadUShort(),
                IntegralTypes.Byte => (T)(object)ReadByte(),
                _ => default,
            };
        }

        public Enum ReadEnum(Type type)
        {
            return ReflectionHelper.GetEnumType(type) switch
            {
                IntegralTypes.Int => (Enum)(object)ReadInt(),
                IntegralTypes.UInt => (Enum)(object)ReadUInt(),
                IntegralTypes.Long => (Enum)(object)ReadLong(),
                IntegralTypes.ULong => (Enum)(object)ReadULong(),
                IntegralTypes.Short => (Enum)(object)ReadShort(),
                IntegralTypes.UShort => (Enum)(object)ReadUShort(),
                IntegralTypes.Byte => (Enum)(object)ReadByte(),
                _ => default,
            };
        }

        #endregion Read And Advance

        #region Peek and Stay

        public int PeekInt()
        {
            return BitConverter.ToInt32(PeekData(sizeof(int)));
        }

        public uint PeekUInt()
        {
            return BitConverter.ToUInt32(PeekData(sizeof(uint)));
        }

        public long PeekLong()
        {
            return BitConverter.ToInt64(PeekData(sizeof(long)));
        }

        public ulong PeekULong()
        {
            return BitConverter.ToUInt64(PeekData(sizeof(ulong)));
        }

        public short PeekShort()
        {
            return BitConverter.ToInt16(PeekData(sizeof(short)));
        }

        public ushort PeekUShort()
        {
            return BitConverter.ToUInt16(PeekData(sizeof(ushort)));
        }

        public byte PeekByte()
        {
            return _buffer.Memory.Span[_position];
        }

        public char PeekChar()
        {
            return BitConverter.ToChar(PeekData(sizeof(char)));
        }

        public bool PeekBool()
        {
            return BitConverter.ToBoolean(PeekData(sizeof(bool)));
        }

        public T PeekEnum<T>() where T : Enum
        {
            return ReflectionHelper.GetEnumType(typeof(T)) switch
            {
                IntegralTypes.Int => (T)(object)PeekInt(),
                IntegralTypes.UInt => (T)(object)PeekUInt(),
                IntegralTypes.Long => (T)(object)PeekLong(),
                IntegralTypes.ULong => (T)(object)PeekULong(),
                IntegralTypes.Short => (T)(object)PeekShort(),
                IntegralTypes.UShort => (T)(object)PeekUShort(),
                IntegralTypes.Byte => (T)(object)PeekByte(),
                _ => default,
            };
        }

        public Enum PeekEnum(Type type)
        {
            return ReflectionHelper.GetEnumType(type) switch
            {
                IntegralTypes.Int => (Enum)(object)PeekInt(),
                IntegralTypes.UInt => (Enum)(object)PeekUInt(),
                IntegralTypes.Long => (Enum)(object)PeekLong(),
                IntegralTypes.ULong => (Enum)(object)PeekULong(),
                IntegralTypes.Short => (Enum)(object)PeekShort(),
                IntegralTypes.UShort => (Enum)(object)PeekUShort(),
                IntegralTypes.Byte => (Enum)(object)PeekByte(),
                _ => default,
            };
        }

        public string PeekString()
        {
            int len = PeekInt();
            Span<char> span = stackalloc char[len];

            for (int i = 0; i < len; i++)
            {
                span[i] = PeekChar();
            }

            return new string(span);
        }

        public object ReadObject(Type type)
        {
            return ReflectionHelper.GetSerializableType(type) switch
            {
                SerializableTypes.Int => ReadInt(),
                SerializableTypes.UInt => ReadUInt(),
                SerializableTypes.Long => ReadLong(),
                SerializableTypes.ULong => ReadULong(),
                SerializableTypes.Short => ReadShort(),
                SerializableTypes.UShort => ReadUShort(),
                SerializableTypes.Byte => ReadByte(),
                SerializableTypes.Char => ReadChar(),
                SerializableTypes.Bool => ReadBool(),
                SerializableTypes.String => ReadString(),
                SerializableTypes.Enum => ReadEnum<Enum>(),
                _ => null,
            };
        }

        #endregion Peek and Stay

        public void Dispose()
        {
            _buffer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void Advance(int amount)
        {
            _position += amount;
        }

        private Span<byte> ReadData(int amount)
        {
            var data = _buffer.Memory.Slice(_position, amount).Span;
            Advance(amount);
            return data;
        }

        private Span<byte> PeekData(int amount)
        {
            return _buffer.Memory.Slice(_position, amount).Span;
        }
    }
}
