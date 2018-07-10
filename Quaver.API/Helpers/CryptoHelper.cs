using System.Security.Cryptography;
using System.Text;

namespace Quaver.API.Helpers
{
    internal static class CryptoHelper
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
    }
}