using System;
using System.IO.Compression;
using System.IO;

namespace CrunchySerialize.Utility
{
    /// <summary>
    /// Helper class used to compress and decompress <see cref="byte"/> arrays
    /// </summary>
    public static class CompressionHelper
    {
        /// <summary>
        /// Compressed the given <see cref="byte"/> array
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="level">The level of compression</param>
        /// <returns>Compressed <see cref="byte"/> array</returns>
        public static byte[] Compress(byte[] data, CompressionLevel level)
        {
            using MemoryStream output = new();
            using (DeflateStream dstream = new(output, level))
            {
                dstream.Write(data);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Compressed the given <see cref="byte"/> array
        /// </summary>
        /// <param name="data">The data to compress</param>
        /// <param name="level">The level of compression</param>
        /// <returns>Compressed <see cref="byte"/> array</returns>
        public static ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> data, CompressionLevel level)
        {
            using MemoryStream output = new();
            using (DeflateStream dstream = new(output, level))
            {
                dstream.Write(data);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Decompressed the given <see cref="byte"/> array
        /// </summary>
        /// <param name="data">The data to decompress</param>
        /// <returns>Decompressed <see cref="byte"/> array</returns>
        public static byte[] Decompress(byte[] data)
        {
            using MemoryStream input = new(data);
            using MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Decompressed the given <see cref="byte"/> array
        /// </summary>
        /// <param name="data">The data to decompress</param>
        /// <returns>Decompressed <see cref="byte"/> array</returns>
        public static ReadOnlySpan<byte> Decompress(ReadOnlySpan<byte> data)
        {
            using MemoryStream input = new(data.ToArray());
            using MemoryStream output = new();
            using (DeflateStream dstream = new(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
