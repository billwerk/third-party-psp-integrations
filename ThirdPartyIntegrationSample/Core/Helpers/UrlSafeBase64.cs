using System;

namespace Core.Helpers
{
    /// <summary>
    /// A Base64 encoding that uses '-' and '_' instead of '+' and '/', thus 
    /// making it safe for use in URLs
    /// </summary>
    public static class UrlSafeBase64
    {
        /// <summary>
        /// Encodes a byte array in a Base64 variant that is URL safe through 
        /// the characters ('-', '_') instead of ('+', '/')
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encode(byte[] input)
        {
            return ToUrlSafeVariant(Convert.ToBase64String(input));
        }

        /// <summary>
        /// Decodes a "UrlSafeVariant Base64" string into a byte[]
        /// </summary>
        /// <param name="base64Encoded"></param>
        /// <returns></returns>
        public static byte[] Decode(string base64Encoded)
        {
            return Convert.FromBase64String(FromUrlSafeVariant(base64Encoded));
        }

        public static string ToUrlSafeVariant(string input)
        {
            return input.Replace('+', '-').Replace('/', '_');
        }

        public static string FromUrlSafeVariant(string input)
        {
            while (input.Length % 4 != 0)
                input += "=";
            return input.Replace('-', '+').Replace('_', '/');
        }
    }
}