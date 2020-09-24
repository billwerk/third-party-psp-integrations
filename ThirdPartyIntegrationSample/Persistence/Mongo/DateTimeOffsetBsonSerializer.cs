using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Persistence.Mongo
{
    // Unlike the default DateTimeOffsetSerializer, this allows us to filter and index the .DateTime and .UtcDateTime properties
    // It can deserialize the default format (LazyMigration) but queries will misbehave on such data
    // Millisecond granularity
    public sealed class DateTimeOffsetBsonSerializer : BsonClassMapSerializer<DateTimeOffset>
    {
        private class DateTimeWithKindSerializer : BsonProxySerializer<DateTime, DateTime>
        {
            private readonly DateTimeKind _kind;

            public DateTimeWithKindSerializer(DateTimeKind kind)
            {
                _kind = kind;
            }

            protected override DateTime FromProxy(DateTime proxyValue)
            {
                if (proxyValue.Kind != DateTimeKind.Utc)
                    throw new InvalidOperationException($"Expected deserialization to produce DateTimeKind.Utc received {proxyValue.Kind}");
                return new DateTime(proxyValue.Ticks, DateTimeKind.Unspecified);
            }

            protected override DateTime ToProxy(DateTime value)
            {
                if (value.Kind != _kind)
                    throw new ArgumentException($"Expected DateTimeKind.{_kind} received {value.Kind}", nameof(DateTime.Kind));
                return new DateTime(value.Ticks, DateTimeKind.Utc);
            }
        }

        private static DateTimeOffset CreateValue(DateTime local, DateTime utc)
        {
            return new DateTimeOffset(local.Ticks, local - utc);
        }

        private static BsonClassMap<DateTimeOffset> CreateClassMap()
        {
            var classMap = new BsonClassMap<DateTimeOffset>();

            var localMap = classMap.MapProperty(x => x.DateTime);
            localMap.SetElementName("L");
            localMap.SetSerializer(new DateTimeWithKindSerializer(DateTimeKind.Unspecified));

            var utcMap = classMap.MapProperty(x => x.UtcDateTime);
            utcMap.SetElementName("U");
            utcMap.SetSerializer(new DateTimeWithKindSerializer(DateTimeKind.Utc));

            // for some reason MapCreator doesn't work for structs, so we have to implement deserialization via a proxy
            // classMap.MapCreator(new Func<DateTime, DateTime, DateTimeOffset>(CreateValue), nameof(DateTimeOffset.DateTime), nameof(DateTimeOffset.UtcDateTime));

            classMap.Freeze();
            return classMap;
        }

        private DateTimeOffsetBsonSerializer()
            : base(BsonClassMap.LookupClassMap(typeof(DateTimeOffset)))
        {
            _proxySerializer = BsonSerializer.LookupSerializer<DateTimeOffsetProxy>();
        }

        private class DateTimeOffsetProxy
        {
            [BsonElement("L")]
            public DateTime DateTime { get; set; }
            [BsonElement("U")]
            public DateTime UtcDateTime { get; set; }
        }

        private readonly IBsonSerializer<DateTimeOffsetProxy> _proxySerializer;
        private static readonly IBsonSerializer<DateTimeOffset> _compatibilitySerializer = new DateTimeOffsetSerializer();

        public override DateTimeOffset Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            switch (context.Reader.GetCurrentBsonType())
            {
                case BsonType.Document:
                    args.NominalType = typeof(DateTimeOffsetProxy);
                    var proxyValue = _proxySerializer.Deserialize(context, args);

                    return CreateValue(proxyValue.DateTime, proxyValue.UtcDateTime);
                case BsonType.Array:
                    return _compatibilitySerializer.Deserialize(context, args);
                default:
                    throw new Exception($"Unexpected BsonType {context.Reader.CurrentBsonType}");
            }
        }

        public static void Register()
        {
            BsonClassMap.RegisterClassMap(CreateClassMap());
            BsonSerializer.RegisterSerializer(new DateTimeOffsetBsonSerializer());
        }
    }
}
