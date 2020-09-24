using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

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
