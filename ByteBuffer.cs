using System;
using System.Buffers;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
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
        public int Length { get; private set; }

        public readonly IMemoryOwner<byte> _buffer;
        private int _position;

        /// <summary>
        /// Create a new instance of <see cref="ByteBuffer"/> filled with <paramref name="data"/>
        /// </summary>
        /// <param name="data">The binary data to write into the buffer</param>
        public ByteBuffer(ReadOnlySpan<byte> data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.ToArray().CopyTo(_buffer.Memory);
            Length = data.Length;
        }

        /// <summary>
        /// Create a new instance of <see cref="ByteBuffer"/> filled with <paramref name="data"/>
        /// </summary>
        /// <param name="data">The binary data to write into the buffer</param>
        public ByteBuffer(ReadOnlyMemory<byte> data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.CopyTo(_buffer.Memory);
            Length = data.Length;
        }

        /// <summary>
        /// Create a new instance of <see cref="ByteBuffer"/> filled with <paramref name="data"/>
        /// </summary>
        /// <param name="data">The binary data to write into the buffer</param>
        public ByteBuffer(byte[] data)
        {
            _buffer = MemoryPool<byte>.Shared.Rent(data.Length);
            data.CopyTo(_buffer.Memory);
            Length = data.Length;
        }

        #region Read And Advance

        #region Single Value

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
        /// Reads a <see cref="byte"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte ReadByte()
        {
            byte data = ReadData(1)[0];
            return data;
        }

        /// <summary>
        /// Reads a <see cref="byte"/> <see cref="ReadOnlySpan{T}"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ReadOnlySpan<byte> ReadSpan(int length)
        {
            Span<byte> data = _buffer.Memory.Span.Slice(_position, length);
            Advance(length);
            return data;
        }

        /// <summary>
        /// Reads a <see cref=""/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public string ReadString()
        {
            return Encoding.Default.GetString(ReadData(ReadInt()));
        }

        /// <summary>
        /// Reads a generic <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="T">The generic <see cref="Enum"/></typeparam>
        public T ReadEnum<T>() where T : Enum
        {
            return (T)(object)(ReflectionHelper.GetEnumType(typeof(T)) switch
            {
                IntegralTypes.Int => ReadInt(),
                IntegralTypes.UInt => ReadUInt(),
                IntegralTypes.Long => ReadLong(),
                IntegralTypes.ULong => ReadULong(),
                IntegralTypes.Short => ReadShort(),
                IntegralTypes.UShort => ReadUShort(),
                IntegralTypes.Byte => ReadByte(),
                _ => default,
            });
        }

        /// <summary>
        /// Reads a <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Enum"/></param>
        public Enum ReadEnum(Type type)
        {
            return (Enum)Enum.ToObject(type, ReflectionHelper.GetEnumType(type) switch
            {
                IntegralTypes.Int => ReadInt(),
                IntegralTypes.UInt => ReadUInt(),
                IntegralTypes.Long => ReadLong(),
                IntegralTypes.ULong => ReadULong(),
                IntegralTypes.Short => ReadShort(),
                IntegralTypes.UShort => ReadUShort(),
                IntegralTypes.Byte => ReadByte(),
                _ => default,
            });
        }

        /// <summary>
        /// Reads a object given the type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="object"/></param>
        public object ReadPrimitive(Type type)
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

        #endregion Single Value

        #region Arrays

        /// <summary>
        /// Reads a <see cref="byte"/> array with given length from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="length">The amount of bytes to read</param>
        public byte[] ReadArray(int length)
        {
            byte[] data = ReadData(length);
            return data;
        }

        /// <summary>
        /// Reads a <see cref="byte"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte[] ReadByteArray()
        {
            int len = ReadInt();
            byte[] data = ReadData(len);
            return data;
        }

        /// <summary>
        /// Reads a <see cref="int"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public int[] ReadIntArray()
        {
            int[] array = new int[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadInt();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="uint"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public uint[] ReadUIntArray()
        {
            uint[] array = new uint[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadUInt();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="long"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public long[] ReadLongArray()
        {
            long[] array = new long[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadLong();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="ulong"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public ulong[] ReadULongArray()
        {
            ulong[] array = new ulong[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadULong();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="short"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public short[] ReadShortArray()
        {
            short[] array = new short[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadShort();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="ushort"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public ushort[] ReadUShortArray()
        {
            ushort[] array = new ushort[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadUShort();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="char"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public char[] ReadCharArray()
        {
            char[] array = new char[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadChar();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="bool"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public bool[] ReadBoolArray()
        {
            bool[] array = new bool[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadBool();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="string"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public string[] ReadStringArray()
        {
            string[] array = new string[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadString();
            }
            return array;
        }

        /// <summary>
        /// Reads a <see cref="Enum"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The type of the <see cref="Enum"/></param>
        public Enum[] ReadEnumArray(Type type)
        {
            Enum[] array = new Enum[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadEnum(type);
            }
            return array;
        }

        /// <summary>
        /// Reads a generic <see cref="Enum"/> <typeparamref name="TEnum"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="TEnum">The type of the <see cref="Enum"/></typeparam>
        public TEnum[] ReadEnumArray<TEnum>() where TEnum : Enum
        {
            TEnum[] array = new TEnum[ReadInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ReadEnum<TEnum>();
            }
            return array;
        }

        #endregion Arrays

        #endregion Read And Advance

        #region Peek and Stay

        #region Single Values

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
        /// Peeks a <see cref="byte"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte PeekByte()
        {
            return PeekData(1)[0];
        }

        /// <summary>
        /// Reads a <see cref="byte"/> <see cref="ReadOnlySpan{T}"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public ReadOnlySpan<byte> PeekSpan(int lenght)
        {
            return _buffer.Memory.Span.Slice(_position, lenght);
        }

        /// <summary>
        /// Peeks a <see cref="string"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        public string PeekString()
        {
            return Encoding.Default.GetString(PeekData(_position + sizeof(int), PeekInt()));
        }

        /// <summary>
        /// Peeks a generic <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="T">The generic <see cref="Enum"/></typeparam>
        public T PeekEnum<T>() where T : Enum
        {
            return (T)(object)(ReflectionHelper.GetEnumType(typeof(T)) switch
            {
                IntegralTypes.Int => PeekInt(),
                IntegralTypes.UInt => PeekUInt(),
                IntegralTypes.Long => PeekLong(),
                IntegralTypes.ULong => PeekULong(),
                IntegralTypes.Short => PeekShort(),
                IntegralTypes.UShort => PeekUShort(),
                IntegralTypes.Byte => PeekByte(),
                _ => default,
            });
        }

        /// <summary>
        /// Peeks a <see cref="Enum"/> from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Enum"/></param>
        public Enum PeekEnum(Type type)
        {
            return (Enum)Enum.ToObject(type, ReflectionHelper.GetEnumType(type) switch
            {
                IntegralTypes.Int => PeekInt(),
                IntegralTypes.UInt => PeekUInt(),
                IntegralTypes.Long => PeekLong(),
                IntegralTypes.ULong => PeekULong(),
                IntegralTypes.Short => PeekShort(),
                IntegralTypes.UShort => PeekUShort(),
                IntegralTypes.Byte => PeekByte(),
                _ => default,
            });
        }

        /// <summary>
        /// Peeks a object given the type <paramref name="type"/>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="object"/></param>
        public object PeekPrimitive(Type type)
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

        #endregion Single Values

        #region Arrays

        /// <summary>
        /// Peeks a <see cref="byte"/> array with given length from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="length">The amount of bytes to peek</param>
        public byte[] PeekArray(int length)
        {
            return PeekData(length);
        }

        /// <summary>
        /// Reads a <see cref="byte"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public byte[] PeekByteArray()
        {
            int len = PeekInt();
            return PeekData(_position + sizeof(int), len);
        }

        /// <summary>
        /// Peeks a <see cref="int"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public int[] PeekIntArray()
        {
            int[] array = new int[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToInt32(PeekData(i * sizeof(int), sizeof(int)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="uint"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public uint[] PeekUIntArray()
        {
            uint[] array = new uint[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToUInt32(PeekData(i * sizeof(uint), sizeof(uint)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="long"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public long[] PeekLongArray()
        {
            long[] array = new long[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToInt64(PeekData(i * sizeof(long), sizeof(long)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="ulong"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public ulong[] PeekULongArray()
        {
            ulong[] array = new ulong[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToUInt64(PeekData(i * sizeof(ulong), sizeof(ulong)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="short"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public short[] PeekShortArray()
        {
            short[] array = new short[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToInt16(PeekData(i * sizeof(short), sizeof(short)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="ushort"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public ushort[] PeekUShortArray()
        {
            ushort[] array = new ushort[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToUInt16(PeekData(i * sizeof(ushort), sizeof(ushort)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="char"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public char[] PeekCharArray()
        {
            char[] array = new char[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToChar(PeekData(i * sizeof(char), sizeof(char)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="bool"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public bool[] PeekBoolArray()
        {
            bool[] array = new bool[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = BitConverter.ToBoolean(PeekData(i * sizeof(bool), sizeof(bool)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="string"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        public string[] PeekStringArray()
        {
            string[] array = new string[PeekInt()];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Encoding.Default.GetString(PeekData(i * sizeof(bool), sizeof(bool)));
            }
            return array;
        }

        /// <summary>
        /// Peeks a <see cref="Enum"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="type">The type of the <see cref="Enum"/></param>
        public Enum[] PeekEnumArray(Type type)
        {
            Enum[] array = new Enum[PeekInt()];
            IntegralTypes baseType = ReflectionHelper.GetEnumType(type);

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (Enum)Enum.ToObject(type, baseType switch
                {
                    IntegralTypes.Int => BitConverter.ToInt32(PeekData(i * sizeof(int), sizeof(int))),
                    IntegralTypes.UInt => BitConverter.ToUInt32(PeekData(i * sizeof(uint), sizeof(uint))),
                    IntegralTypes.Long => BitConverter.ToInt64(PeekData(i * sizeof(long), sizeof(long))),
                    IntegralTypes.ULong => BitConverter.ToUInt64(PeekData(i * sizeof(ulong), sizeof(ulong))),
                    IntegralTypes.Short => BitConverter.ToInt16(PeekData(i * sizeof(short), sizeof(short))),
                    IntegralTypes.UShort => BitConverter.ToUInt16(PeekData(i * sizeof(ushort), sizeof(ushort))),
                    IntegralTypes.Byte => PeekData(i, 1)[0],
                    _ => default,
                });
            }
            return array;
        }

        /// <summary>
        /// Reads a generic <see cref="Enum"/> <typeparamref name="TEnum"/> array from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <typeparam name="TEnum">The type of the <see cref="Enum"/></typeparam>
        public TEnum[] PeekEnumArray<TEnum>() where TEnum : Enum
        {
            TEnum[] array = new TEnum[ReadInt()];
            Type type = typeof(TEnum);
            IntegralTypes baseType = ReflectionHelper.GetEnumType(type);

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (TEnum)Enum.ToObject(type, baseType switch
                {
                    IntegralTypes.Int => BitConverter.ToInt32(PeekData(i * sizeof(int), sizeof(int))),
                    IntegralTypes.UInt => BitConverter.ToUInt32(PeekData(i * sizeof(uint), sizeof(uint))),
                    IntegralTypes.Long => BitConverter.ToInt64(PeekData(i * sizeof(long), sizeof(long))),
                    IntegralTypes.ULong => BitConverter.ToUInt64(PeekData(i * sizeof(ulong), sizeof(ulong))),
                    IntegralTypes.Short => BitConverter.ToInt16(PeekData(i * sizeof(short), sizeof(short))),
                    IntegralTypes.UShort => BitConverter.ToUInt16(PeekData(i * sizeof(ushort), sizeof(ushort))),
                    IntegralTypes.Byte => PeekData(i, 1)[0],
                    _ => default,
                });
            }
            return array;
        }

        #endregion Arrays

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
        public ReadOnlySpan<byte> CompressIntoSpan(CompressionLevel level = CompressionLevel.Optimal)
        {
            return CompressionHelper.Compress(_buffer.Memory.Span, level);
        }

        /// <summary>
        /// Compressed data from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <param name="level">The level of compression</param>
        /// <returns>Compressed binary array</returns>
        public byte[] CompressIntoArray(CompressionLevel level = CompressionLevel.Optimal)
        {
            return CompressionHelper.Compress(_buffer.Memory.ToArray(), level);
        }

        #endregion Compression

        /// <summary>
        /// Gets the data from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <returns>Binary <see cref="ReadOnlySpan{T}"/></returns>
        public ReadOnlySpan<byte> GetByteSpan()
        {
            return _buffer.Memory.Span.Slice(0, Length);
        }

        /// <summary>
        /// Gets the data from this <see cref="ByteBuffer"/>
        /// </summary>
        /// <returns>Binary array</returns>
        public byte[] GetByteArray()
        {
            return PeekData(0, Length);
        }

        public void Dispose()
        {
            _buffer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void Advance(int amount)
        {
            _position += amount;
            Length += amount;
        }

        private byte[] ReadData(int amount)
        {
            var data = _buffer.Memory.Span.Slice(_position, amount).ToArray();
            Advance(amount);
            return data;
        }

        private byte[] PeekData(int offset, int amount)
        {
            return _buffer.Memory.Span.Slice(offset, amount).ToArray();
        }

        private byte[] PeekData(int amount)
        {
            return _buffer.Memory.Span.Slice(_position, amount).ToArray();
        }
    }
}
