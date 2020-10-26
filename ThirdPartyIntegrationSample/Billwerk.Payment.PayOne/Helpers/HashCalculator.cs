using System.Security.Cryptography;
using System.Text;

namespace Billwerk.Payment.PayOne.Helpers
{
    public static class HashCalculator
    {
        public static string GetMd5(string str)
        {
            var bytes = MD5.Create().ComputeHash(new ASCIIEncoding().GetBytes(str));

            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString().ToLower();
        }
    }
}