using System;
using Core.Helpers;
using MongoDB.Bson;
using Persistence.Interfaces;

namespace Persistence.Mongo
{
    public static class ObjectIdExtensions
    {
        public static ObjectId<T> GetId<T>(this T obj)
            where T : IDatabaseObject
        {
            CodeContract.Requires<ArgumentNullException>(obj != null, "obj != null");
            return obj.Id.AsTyped<T>();
        }

        public static ObjectId<T> AsTyped<T>(this ObjectId untyped)
            where T : IDatabaseObject
        {
            return new ObjectId<T>(untyped);
        }

        public static ObjectId<T>? AsTyped<T>(this ObjectId? untyped)
            where T : IDatabaseObject
        {
            if (untyped == null)
                return null;
            return new ObjectId<T>(untyped.Value);
        }
    }
}
