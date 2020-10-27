using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.Interfaces;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Models
{
    [BsonDiscriminator(Required = true)]
    [BsonKnownTypes(typeof(RecurringTokenPayOne))]
    public abstract class RecurringToken : DbObject, IRecurringToken
    {
        [BsonIgnore]
        public string Token => Id.ToString();
    }
}