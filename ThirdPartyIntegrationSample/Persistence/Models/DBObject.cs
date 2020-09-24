using System;
using Core.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Persistence.Interfaces;
using Persistence.Mongo;

namespace Persistence.Models
{
    public abstract class DbObject : IDatabaseObject
    {
        /// <summary>
        /// The [BsonIgnoreIfDefault] attribute is important so an Update.Replace in an upsert operation behaves
        /// the way it should!
        /// </summary>
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; private set; }

        private void ForceId(ObjectId id) => Id = id;

        public void ForceId()
        {
            CodeContract.Requires<InvalidOperationException>(Id == ObjectId.Empty, "Id == ObjectId.Empty");
            var id = ObjectId.GenerateNewId();
            ForceId(id);
        }

        public override string ToString() => $"{base.ToString()} {Id}";

        static DbObject()
        {
            RegisterBsonSerializers.EnsureRegistered();
        }
    }
}