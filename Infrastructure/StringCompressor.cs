using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication25.Infrastructure
{
    internal static class StringCompressor
    {
        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string value)
        {
            //Transform string into byte[] 
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
            System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();

            return sB.ToString();
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string sData)
        {
            byte[] byteArray = new byte[sData.Length];

            int indexBa = 0;
            foreach (char item in sData)
                byteArray[indexBa++] = (byte)item;

            MemoryStream memoryStream = new MemoryStream(byteArray);
            GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);

            byteArray = new byte[1024];

            StringBuilder stringBuilder = new StringBuilder();

            int readBytes;
            while ((readBytes = gZipStream.Read(byteArray, 0, byteArray.Length)) != 0)
            {
                for (int i = 0; i < readBytes; i++) stringBuilder.Append((char)byteArray[i]);
            }
            gZipStream.Close(); memoryStream.Close(); gZipStream.Dispose(); memoryStream.Dispose(); return stringBuilder.ToString();
        }

    }
}
