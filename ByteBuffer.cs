using System;
using System.Buffers;
using System.IO.Compression;
using CrunchySerialize.Utility;

namespace CrunchySerialize
{
    /// <summary>
    /// Class used for reading stored binary data
    /// </summary>
    public sealed class ByteBuffer : IDisposable
    {
        /// <summary>
        /// The amount of data inside this <see cref="ByteBuffer"/>
        /// </summary>
        public int Length { get => _buffer.Memory.Length; }

        public ByteWriter Writer { get; }

        private readonly IMemoryOwner<byte> _buffer;
        private int _position;

        /// <summary>
        /// Creates a new empty instance of <see cref="ByteBuffer"/>
        /// </summary>
        [Obsolete("Please use the other constructor for ByteBuffer as there is now way to write data directly into it after construction")]
        public ByteBuffer()
        {
            _buffer = MemoryPool<byte>.Shared.Rent();
        }

        /// <summary>
        /// Create a new instance of <see cref="ByteBuffer"/> filled with <paramref name="data"/>
        /// </summary>
        /// <param name="data">The binary data to write into the buffer</param>
        public ByteBuffer(ReadOnlySpan<byte> data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.ToArray().CopyTo(_buffer.Memory);
        }

        /// <summary>
        /// Create a new instance of <see cref="ByteBuffer"/> filled with <paramref name="data"/>
        /// </summary>
        /// <param name="data">The binary data to write into the buffer</param>
        public ByteBuffer(byte[] data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.CopyTo(_buffer.Memory);
        }

        #region Read And Advance

        /// <summary>
        /// Reads a <see cref="int"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public int ReadInt()
        {
            return BitConverter.ToInt32(ReadData(sizeof(int)));
        }

        /// <summary>
        /// Reads a <see cref="uint"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(ReadData(sizeof(uint)));
        }

        /// <summary>
        /// Reads a <see cref="long"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public long ReadLong()
        {
            return BitConverter.ToInt64(ReadData(sizeof(long)));
        }

        /// <summary>
        /// Reads a <see cref="ulong"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ulong ReadULong()
        {
            return BitConverter.ToUInt64(ReadData(sizeof(ulong)));
        }

        /// <summary>
        /// Reads a <see cref="short"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public short ReadShort()
        {
            return BitConverter.ToInt16(ReadData(sizeof(short)));
        }

        /// <summary>
        /// Reads a <see cref="ushort"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(ReadData(sizeof(ushort)));
        }

        /// <summary>
        /// Reads a <see cref="byte"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte ReadByte()
        {
            byte data = _buffer.Memory.Span[_position];
            Advance(1);
            return data;
        }

        /// <summary>
        /// Reads a <see cref="char"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public char ReadChar()
        {
            return BitConverter.ToChar(ReadData(sizeof(char)));
        }

        /// <summary>
        /// Reads a <see cref="bool"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public bool ReadBool()
        {
            return BitConverter.ToBoolean(ReadData(sizeof(bool)));
        }

        /// <summary>
        /// Reads a <see cref=""/> from this <see cref="ByteBuffer"/>
        /// </summary>
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

        /// <summary>
        /// Reads a generic <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="T">The generic <see cref="Enum"/></typeparam>
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

        /// <summary>
        /// Reads a <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Enum"/></param>
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

        /// <summary>
        /// Reads a object given the type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="object"/></param>
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

        #endregion Read And Advance

        #region Peek and Stay

        /// <summary>
        /// Peeks a <see cref="int"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public int PeekInt()
        {
            return BitConverter.ToInt32(PeekData(sizeof(int)));
        }

        /// <summary>
        /// Peeks a <see cref="uint"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public uint PeekUInt()
        {
            return BitConverter.ToUInt32(PeekData(sizeof(uint)));
        }

        /// <summary>
        /// Peeks a <see cref="long"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public long PeekLong()
        {
            return BitConverter.ToInt64(PeekData(sizeof(long)));
        }

        /// <summary>
        /// Peeks a <see cref="ulong"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ulong PeekULong()
        {
            return BitConverter.ToUInt64(PeekData(sizeof(ulong)));
        }

        /// <summary>
        /// Peeks a <see cref="short"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public short PeekShort()
        {
            return BitConverter.ToInt16(PeekData(sizeof(short)));
        }

        /// <summary>
        /// Peeks a <see cref="ushort"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ushort PeekUShort()
        {
            return BitConverter.ToUInt16(PeekData(sizeof(ushort)));
        }

        /// <summary>
        /// Peeks a <see cref="byte"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte PeekByte()
        {
            return _buffer.Memory.Span[_position];
        }

        /// <summary>
        /// Peeks a <see cref="char"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public char PeekChar()
        {
            return BitConverter.ToChar(PeekData(sizeof(char)));
        }

        /// <summary>
        /// Peeks a <see cref="bool"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public bool PeekBool()
        {
            return BitConverter.ToBoolean(PeekData(sizeof(bool)));
        }

        /// <summary>
        /// Peeks a <see cref="string"/> from this <see cref="ByteBuffer"/>
        /// </summary>
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

        /// <summary>
        /// Peeks a generic <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="T">The generic <see cref="Enum"/></typeparam>
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

        /// <summary>
        /// Peeks a <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Enum"/></param>
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

        /// <summary>
        /// Peeks a object given the type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="object"/></param>
        public object PeekObject(Type type)
        {
            return ReflectionHelper.GetSerializableType(type) switch
            {
                SerializableTypes.Int => PeekInt(),
                SerializableTypes.UInt => PeekUInt(),
                SerializableTypes.Long => PeekLong(),
                SerializableTypes.ULong => PeekULong(),
                SerializableTypes.Short => PeekShort(),
                SerializableTypes.UShort => PeekUShort(),
                SerializableTypes.Byte => PeekByte(),
                SerializableTypes.Char => PeekChar(),
                SerializableTypes.Bool => PeekBool(),
                SerializableTypes.String => PeekString(),
                SerializableTypes.Enum => PeekEnum<Enum>(),
                _ => null,
            };
        }

        #endregion Peek and Stay

        #region Compression

        /// <summary>
        /// Decompressed and writes data into a new instance of <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="data">The compressed binary data</param>
        /// <returns><see cref="ByteBuffer"/> containing the decompressed data</returns>
        public static ByteBuffer FromCompressed(byte[] data)
        {
            return new ByteBuffer(CompressionHelper.Decompress(data));
        }

        /// <summary>
        /// Decompressed and writes data into a new instance of <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="data">The compressed binary data</param>
        /// <returns><see cref="ByteBuffer"/> containing the decompressed data</returns>
        public static ByteBuffer FromCompressed(ReadOnlySpan<byte> data)
        {
            return new ByteBuffer(CompressionHelper.Decompress(data));
        }

        /// <summary>
        /// Compressed data from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="level">The level of compression</param>
        /// <returns>Compressed binary <see cref="ReadOnlySpan{T}"/></returns>
        public ReadOnlySpan<byte> CompressData(CompressionLevel level = CompressionLevel.Optimal)
        {
            return CompressionHelper.Compress(_buffer.Memory.Span, level);
        }

        #endregion Compression

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
