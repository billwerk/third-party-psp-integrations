using System;
using System.Text;

namespace Billwerk.Payment.PayOne.Helpers
{
    public static class Base32
    {
        private static readonly char[] Digits;
        private static readonly int Mask;
        private static readonly int Shift;

        static Base32()
        {
            Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
            Mask = Digits.Length - 1;
            Shift = NumberOfTrailingZeros(Digits.Length);
        }

        public static string Encode(byte[] data, bool padOutput = false)
        {
            if (data.Length == 0)
            {
                return "";
            }

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            if (data.Length >= (1 << 28))
            {
                // The computation below will fail, so don't do it.
                throw new ArgumentOutOfRangeException("data");
            }

            var outputLength = (data.Length * 8 + Shift - 1) / Shift;
            var result = new StringBuilder(outputLength);

            int buffer = data[0];
            var next = 1;
            var bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < Shift)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= (data[next++] & 0xff);
                        bitsLeft += 8;
                    }
                    else
                    {
                        var pad = Shift - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }

                var index = Mask & (buffer >> (bitsLeft - Shift));
                bitsLeft -= Shift;
                result.Append(Digits[index]);
            }

            if (!padOutput) return result.ToString();
            var padding = 8 - (result.Length % 8);
            if (padding > 0) result.Append(new string('=', padding == 8 ? 0 : padding));
            return result.ToString();
        }
        
        private static int NumberOfTrailingZeros(int i)
        {
            // HD, Figure 5-14
            if (i == 0) return 32;
            var n = 31;
            var y = i << 16;
            if (y != 0)
            {
                n -= 16;
                i = y;
            }

            y = i << 8;
            if (y != 0)
            {
                n -= 8;
                i = y;
            }

            y = i << 4;
            if (y != 0)
            {
                n -= 4;
                i = y;
            }

            y = i << 2;
            if (y == 0) return n - (int) ((uint) (i << 1) >> 31);
            n -= 2;
            i = y;
            return n - (int) ((uint) (i << 1) >> 31);
        }
    }
}