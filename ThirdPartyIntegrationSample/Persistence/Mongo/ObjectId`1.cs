using System;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Persistence.Interfaces;

namespace Persistence.Mongo
{
    [DebuggerDisplay("{" + nameof(DebugString) + "()}")]
    public readonly struct ObjectId<T> : IEquatable<ObjectId<T>>, IComparable<ObjectId<T>>
        where T : IDatabaseObject
    {
        public ObjectId Untyped { get; }

        public ObjectId(ObjectId untyped)
        {
            Untyped = untyped;
        }

        public override string ToString()
        {
            return Untyped.ToString();
        }

        public override int GetHashCode()
        {
            return Untyped.GetHashCode();
        }

        public bool Equals(ObjectId<T> other)
        {
            return Untyped.Equals(other.Untyped);
        }

        public override bool Equals(object obj)
        {
            return obj is ObjectId<T> other && Equals(other);
        }

        public static bool operator ==(ObjectId<T> x, ObjectId<T> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ObjectId<T> x, ObjectId<T> y)
        {
            return !x.Equals(y);
        }

        public static bool operator <(ObjectId<T> x, ObjectId<T> y)
        {
            return x.Untyped < y.Untyped;
        }

        public static bool operator >(ObjectId<T> x, ObjectId<T> y)
        {
            return x.Untyped > y.Untyped;
        }

        public static bool operator <=(ObjectId<T> x, ObjectId<T> y)
        {
            return x.Untyped <= y.Untyped;
        }

        public static bool operator >=(ObjectId<T> x, ObjectId<T> y)
        {
            return x.Untyped >= y.Untyped;
        }

        public static ObjectId<T> Empty => ObjectId.Empty.AsTyped<T>();

        public string DebugString()
        {
            return $"{typeof(T).Name}Id {Untyped}";
        }

        public static ObjectId<T> Parse(string s)
        {
            return ObjectId.Parse(s).AsTyped<T>();
        }

        public DateTime CreationTime => Untyped.CreationTime;

        public int CompareTo(ObjectId<T> other)
        {
            return Untyped.CompareTo(other.Untyped);
        }

        public bool IsNotEmpty() => this != Empty;
        public bool IsEmpty() => this == Empty;

        public static explicit operator ObjectId(ObjectId<T> id)
        {
            return id.Untyped;
        }
    }

    public class ObjectIdTBsonSerializer<T> : BsonProxySerializer<ObjectId<T>, ObjectId>
        where T : IDatabaseObject
    {
        protected override ObjectId ToProxy(ObjectId<T> value)
        {
            return value.Untyped;
        }

        protected override ObjectId<T> FromProxy(ObjectId proxyValue)
        {
            return new ObjectId<T>(proxyValue);
        }
    }

    public class ObjectIdTBsonSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectId<>))
            {
                return (IBsonSerializer)Activator.CreateInstance(typeof(ObjectIdTBsonSerializer<>).MakeGenericType(type.GenericTypeArguments[0]));
            }
            return null;
        }
    }
}