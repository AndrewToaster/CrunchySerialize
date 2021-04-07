using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrunchySerialize.Utility
{
    public static class CompressionHelper
    {
        public static byte[] Compress(byte[] data, CompressionLevel level)
        {
            using MemoryStream output = new();
            using (DeflateStream dstream = new(output, level))
            {
                dstream.Write(data);
            }
            return output.ToArray();
        }

        public static ReadOnlySpan<byte> Compress(ReadOnlySpan<byte> data, CompressionLevel level)
        {
            using MemoryStream output = new();
            using (DeflateStream dstream = new(output, level))
            {
                dstream.Write(data);
            }
            return output.ToArray();
        }

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
