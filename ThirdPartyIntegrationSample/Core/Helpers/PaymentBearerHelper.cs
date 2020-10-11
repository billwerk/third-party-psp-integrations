using System;

namespace Core.Helpers
{
    public static class PaymentBearerHelper
    {
        public static DateTime GetExpiryDate(int expiryMonth, int expiryYear)
        {
            try
            {
                return CreateUtc(expiryYear, expiryMonth, 1).AddMonths(1);
            }
            catch
            {
                expiryYear = 9999;
                expiryMonth = 9;
                return CreateUtc(expiryYear, expiryMonth, 1);
            }
        }
        
        private static DateTime CreateUtc(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}