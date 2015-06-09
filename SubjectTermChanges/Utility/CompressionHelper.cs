using System.Text;
using System.IO;
using System.IO.Compression;

namespace Kindred.Knect.ITAT.Utility
{
    public static class CompressionHelper
    {
        private const int COMPRESSION_FACTOR = 10;

        public static byte[] Compress(string originalString)
        {
            try
            {
                using (MemoryStream compressedMemoryStream = new MemoryStream())
                {
                    using (DeflateStream compressedZipStream = new DeflateStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                        byte[] originalBytes = UTF8Encoding.UTF8.GetBytes(originalString);
                        compressedZipStream.Write(originalBytes, 0, originalBytes.Length);
                    }
                    //The remaining compressed data and footer info is written out to MemoryStream during GZipStream.Dispose.
                    //Therefore it is required that the GZipStream be closed before reading out the MemoryStream array.
                    //Refer to:  http://blogs.msdn.com/b/bclteam/archive/2006/05/10/592551.aspx
                    return compressedMemoryStream.ToArray();
                }
            }
            catch
            {
                throw;
            }
        }

        public static string Decompress(byte[] compressedBytes)
        {
            try
            {
                using (MemoryStream decompressedMemoryStream = new MemoryStream(compressedBytes))
                {
                    using (DeflateStream decompressedZipStream = new DeflateStream(decompressedMemoryStream, CompressionMode.Decompress))
                    {
                        byte[] decompressionBuffer = new byte[COMPRESSION_FACTOR * compressedBytes.Length];
                        int decompressionBufferByteCount = decompressedZipStream.Read(decompressionBuffer, 0, COMPRESSION_FACTOR * compressedBytes.Length);
                        return UTF8Encoding.UTF8.GetString(decompressionBuffer, 0, decompressionBufferByteCount);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

