using System;

namespace Billwerk.Payment.PayOne.Helpers
{
    public static class DateTimeHelper
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        /// <summary>
        /// Returns the number of SECONDS since unix epoch
        /// </summary>
        public static long ToUnixTime(this DateTime t)
        {
            return (long)t.ToUniversalTime().Subtract(Epoch).TotalSeconds;
        }
    }
}