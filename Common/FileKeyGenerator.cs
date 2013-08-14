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
        public static string GenerateKey(string filePath)
        {
            using (BufferedStream fileStream = new BufferedStream(File.Open(filePath, FileMode.Open)))
            {
                return GenerateKey(fileStream);
            }
        }

        public static string GenerateKey(Stream fileStream)
        {
            MD5 hasher = MD5.Create();
            byte[] hash = hasher.ComputeHash(fileStream);
            return Encoding.Default.GetString(hash);
        }
    }
}