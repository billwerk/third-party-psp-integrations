using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Persistence.Mongo
{
    public abstract class BsonProxySerializer<T, TProxy> : SerializerBase<T>
    {
        private readonly Lazy<IBsonSerializer> _proxySerializer = new Lazy<IBsonSerializer>(() => BsonSerializer.LookupSerializer(typeof(TProxy)));

        protected abstract TProxy ToProxy(T value);
        protected abstract T FromProxy(TProxy proxyValue);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            args.NominalType = typeof(TProxy);
            var proxyValue = (TProxy)_proxySerializer.Value.Deserialize(context, args);
            var value = FromProxy(proxyValue);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
        {
            args.NominalType = typeof(TProxy);
            var proxyValue = ToProxy(value);
            _proxySerializer.Value.Serialize(context, args, proxyValue);
        }
    }
}
