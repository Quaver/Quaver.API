using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Quaver.API.Helpers
{
    public static class CryptoHelper
    {
        /// <summary>
        ///     MD5 hash a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StringToMd5(string input)
        {
            // Use input string to calculate MD5 hash
            var sb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        /// <summary>
        ///     Gets the Md5 Checksum of a file, more specifically a .qua file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FileToMd5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }
    }
}