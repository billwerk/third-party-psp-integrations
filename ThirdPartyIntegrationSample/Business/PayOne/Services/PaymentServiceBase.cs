using System;
using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;

namespace Business.PayOne.Services
{
    public abstract class PaymentServiceBase
    {
        protected static T GetPspSettings<T>(IList<MerchantSettingValue> merchantSettings) 
            where T : PSPSettings
        {
            return Activator.CreateInstance(typeof(T), merchantSettings) as T;
        }
        
        protected static string ConvertToLongCardType(string cardType)
        {
            return cardType switch
            {
                "V" => "Visa",
                "M" => "Mastercard",
                "A" => "Amex",
                "D" => "Diners",
                "J" => "JCB",
                "O" => "Maestro International",
                "U" => "Maestro UK",
                "C" => "Discover",
                "B" => "Carte Bleue",
                _ => ""
            };
        }
    }
}