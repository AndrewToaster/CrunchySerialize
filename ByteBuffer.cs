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
                SerializableType.Int => ReadInt(),
                SerializableType.UInt => ReadUInt(),
                SerializableType.Long => ReadLong(),
                SerializableType.ULong => ReadULong(),
                SerializableType.Short => ReadShort(),
                SerializableType.UShort => ReadUShort(),
                SerializableType.Byte => ReadByte(),
                SerializableType.Char => ReadChar(),
                SerializableType.Bool => ReadBool(),
                SerializableType.String => ReadString(),
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
