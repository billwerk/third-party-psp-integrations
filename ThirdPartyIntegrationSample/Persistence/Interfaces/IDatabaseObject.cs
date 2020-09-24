using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistence.Interfaces
{
    public interface IDatabaseObject<out TId>
    {
        [BsonIgnoreIfDefault]
        TId Id { get; }
    }

    public interface IDatabaseObject: IDatabaseObject<ObjectId>
    {
    }
}