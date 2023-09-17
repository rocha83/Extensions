using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Rochas.Extensions.Helpers
{
    internal static class Compressor
    {
        internal static GZipStream? gzipStream = null;
        internal static MemoryStream? memSource = null;
        internal static MemoryStream? memDestination = null;

        public static string ZipText(string rawText)
        {
            byte[] rawBinary = Encoding.ASCII.GetBytes(rawText);

            byte[] compressedBinary = ZipBinary(rawBinary);
            return Convert.ToBase64String(compressedBinary);
        }

        public static string UnZipText(string compressedText)
        {
            byte[] compressedBinary = Convert.FromBase64String(compressedText);
            byte[] destinBinary = UnZipBinary(compressedBinary);

            string result = new string(Encoding.ASCII.GetChars(destinBinary));

            return result.ToString();
        }

        internal static byte[] ZipBinary(byte[] rawSource)
        {
            memDestination = new MemoryStream();
            memSource = new MemoryStream(rawSource);
            gzipStream = new GZipStream(memDestination, CompressionMode.Compress);

            memSource.CopyTo(gzipStream);

            gzipStream.Close();

            return memDestination.ToArray();
        }

        internal static byte[] UnZipBinary(byte[] compressedSource)
        {
            var unpackedContentStream = new MemoryStream(compressedSource);
            byte[] unpackedContent = unpackedContentStream.ToArray();
            memSource = new MemoryStream(compressedSource);

            gzipStream = new GZipStream(memSource, CompressionMode.Decompress);

            var readedBytes = gzipStream.Read(unpackedContent, 0, unpackedContent.Length);

            memDestination = new MemoryStream(unpackedContent, 0, readedBytes);

            return memDestination.ToArray();
        }
    }
}
