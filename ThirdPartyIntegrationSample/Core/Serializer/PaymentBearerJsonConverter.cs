using System;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Serializer
{
    public class PaymentBearerJsonConverter : JsonConverter<PaymentBearerDTO>
    {
        public override void WriteJson(JsonWriter writer, PaymentBearerDTO value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override PaymentBearerDTO ReadJson(JsonReader reader, Type objectType, PaymentBearerDTO existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);
            PaymentBearerDTO obj;

            var bearerType = (jObject[nameof(PaymentBearerDTO.Type)] ?? jObject[nameof(PaymentBearerDTO.Type).ToLower()]).ToString();
            if (bearerType == PaymentBearerType.CreditCard.ToString())
            {
                obj = new PaymentBearerCreditCardDTO();
            }
            else if(bearerType == PaymentBearerType.BankAccount.ToString())
            {
                obj = new PaymentBearerBankAccountDTO();
            }
            else
            {
                throw new NotImplementedException($"BearerType={bearerType} is not supported");
            }
            

            serializer.Populate(jObject.CreateReader(), obj);

            return obj;
        }

        public override bool CanWrite => false;
    }
}