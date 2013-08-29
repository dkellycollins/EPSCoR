using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace EPSCoR.Common
{
    public class FileKeyGenerator
    {
        /// <summary>
        /// Generates a unique key based on the file and it contents.
        /// </summary>
        /// <param name="filePath">The fully qualified path the file.</param>
        /// <returns>Unique file key.</returns>
        public static string GenerateKey(string filePath)
        {
            using (BufferedStream fileStream = new BufferedStream(File.Open(filePath, FileMode.Open)))
            {
                return GenerateKey(fileStream);
            }
        }

        /// <summary>
        /// Generates a unique key based on the file and it contents.
        /// </summary>
        /// <param name="fileStream">The open file stream.</param>
        /// <returns>Unique file key.</returns>
        public static string GenerateKey(Stream fileStream)
        {
            MD5 hasher = MD5.Create();
            byte[] hash = hasher.ComputeHash(fileStream);
            return Encoding.Default.GetString(hash);
        }
    }
}