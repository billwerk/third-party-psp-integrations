using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Mongo;

namespace Persistence.Helpers
{
    public static class MongoQuery
    {
        public static IMongoQuery TypeEq(string value)
        {
            return Query.EQ(MongoContext.TypeBsonDiscriminator, value);
        }

        public static IMongoQuery TypeNotEq(string value)
        {
            return Query.NE(MongoContext.TypeBsonDiscriminator, value);
        }
    }
}