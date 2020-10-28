using System;

namespace Persistence.Mongo
{
    public class BsonProxyDelegateSerializer<T, TProxy> : BsonProxySerializer<T, TProxy>
    {
        private readonly Func<T, TProxy> _toProxy;
        private readonly Func<TProxy, T> _fromProxy;

        public BsonProxyDelegateSerializer(Func<T, TProxy> toProxy, Func<TProxy, T> fromProxy)
        {
            _toProxy = toProxy;
            _fromProxy = fromProxy;
        }

        protected override T FromProxy(TProxy proxyValue)
        {
            return _fromProxy(proxyValue);
        }

        protected override TProxy ToProxy(T value)
        {
            return _toProxy(value);
        }
    }
}
