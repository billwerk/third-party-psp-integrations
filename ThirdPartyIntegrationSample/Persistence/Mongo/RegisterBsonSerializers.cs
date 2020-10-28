using System;
using Billwerk.Payment.SDK.DTO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using NodaTime;

namespace Persistence.Mongo
{
    public static class RegisterBsonSerializers
    {
        private static void RegisterTypes()
        {
            var pack = new ConventionPack
            {
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(
                "Sloppy Conventions",
                pack,
                t => t.FullName != null && t.FullName.StartsWith("Persistence."));
            

            DateTimeOffsetBsonSerializer.Register();
            BsonSerializer.RegisterSerializationProvider(new ObjectIdTBsonSerializationProvider());
            
            BsonClassMap.RegisterClassMap<PaymentBearerCreditCardDTO>();
            BsonClassMap.RegisterClassMap<PaymentBearerBankAccountDTO>();

            BsonSerializer.RegisterSerializer(typeof(LocalDate), new BsonProxyDelegateSerializer<LocalDate, DateTime>(
                x => new DateTime(x.ToDateTimeUnspecified().Ticks, DateTimeKind.Utc),
                x =>
                {
                    if (x != x.Date)
                        throw new Exception("Serialized LocalDate must be at midnight");
                    return LocalDate.FromDateTime(x);
                }
            ));

            BsonSerializer.RegisterSerializer(typeof(LocalTime), new BsonProxyDelegateSerializer<LocalTime, long>(
                x => x.TickOfDay,
                LocalTime.FromTicksSinceMidnight
            ));
        }

        static RegisterBsonSerializers()
        {
            RegisterTypes();
        }

        // makes sure the static constructor has run
        public static void EnsureRegistered()
        {
        }
    }
}
