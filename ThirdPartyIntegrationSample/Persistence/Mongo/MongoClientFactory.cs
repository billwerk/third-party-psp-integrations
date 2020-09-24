using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;

namespace Persistence.Mongo
{
    public static class MongoClientFactory
    {
        public static IMongoClient Create(string host)
        {
            var mongoClient = new MongoClient($"mongodb://{host}?safe=true");

            ConventionRegistry.Register(
               "Custom Conventions",
               new ConventionPack {
                               new IgnoreExtraElementsConvention(true),
                               new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfDocuments)
               },
                t => t.FullName != null && t.FullName.StartsWith("Persistence."));

            return mongoClient;
        }
    }
}
