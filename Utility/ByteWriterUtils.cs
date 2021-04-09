using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Not in '.Utily' due to the fact it would be hidden when importing only 'CrunchySerialize.ByteWriter'
namespace CrunchySerialize
{
    /// <summary>
    /// Helper class containing extension method for <see cref="ByteWriter"/>
    /// </summary>
    public static class ByteWriterUtils
    {
        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteInt(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, uint[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteUInt(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, long[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteLong(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, ulong[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteULong(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, short[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteShort(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, ushort[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteUShort(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, char[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteChar(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, bool[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteBool(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray(this ByteWriter writer, Enum[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteEnum(array[i]);
            }
        }

        /// <summary>
        /// Writes a primitive array into the internal buffer
        /// </summary>
        /// <param name="writer">The buffer which to write into</param>
        /// <param name="array">The primitive array</param>
        public static void WritePrimitiveArray<TEnum>(this ByteWriter writer, TEnum[] array) where TEnum : Enum
        {
            for (int i = 0; i < array.Length; i++)
            {
                writer.WriteEnum(array[i]);
            }
        }
    }
}
