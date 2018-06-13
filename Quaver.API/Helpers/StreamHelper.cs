using System.IO;

namespace Quaver.API.Helpers
{
    internal static class StreamHelper
    {
        /// <summary>
        ///     Turns a Stream object into a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static byte[] ConvertStreamToByteArray(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
