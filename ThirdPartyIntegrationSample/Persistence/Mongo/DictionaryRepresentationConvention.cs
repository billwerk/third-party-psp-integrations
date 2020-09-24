using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using Persistence.Models;

namespace Persistence.Mongo
{
    public class DictionaryRepresentationConvention : ConventionBase, IMemberMapConvention
    {
        private readonly DictionaryRepresentation _dictionaryRepresentation;
        public DictionaryRepresentationConvention(DictionaryRepresentation dictionaryRepresentation)
        {
            _dictionaryRepresentation = dictionaryRepresentation;
        }

        public void Apply(BsonMemberMap memberMap)
        {
            if (memberMap.MemberType == typeof(ExceptionData))
                return;

            memberMap.SetSerializer(ConfigureSerializer(memberMap.GetSerializer()));
        }

        private IBsonSerializer ConfigureSerializer(IBsonSerializer serializer)
        {
            if (serializer is IDictionaryRepresentationConfigurable dictionarySerializer)
            {
                serializer = dictionarySerializer.WithDictionaryRepresentation(_dictionaryRepresentation);
            }

            var childSerializerConfigurable = serializer as IChildSerializerConfigurable;
            return childSerializerConfigurable == null
                ? serializer
                : childSerializerConfigurable.WithChildSerializer(ConfigureSerializer(childSerializerConfigurable.ChildSerializer));
        }
    }
}
