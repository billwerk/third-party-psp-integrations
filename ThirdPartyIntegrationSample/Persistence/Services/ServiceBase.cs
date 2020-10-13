using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Interfaces;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class ServiceBase<T> : IServiceBase<T> where T : class, IDatabaseObject
    {
        protected readonly IMongoContext Db;

        protected ServiceBase(IMongoContext db)
        {
            Db = db;
        }

        public T Create(T item)
        {
            Db.Insert(item);
            return item;
        }

        public void Update(T item)
        {
            var updated = Db.Update(item);
            if (updated != 1) 
                throw new Exception($"Failed to update object {item.GetType().Name} {item.Id}");
        }

        public T SingleByIdOrDefault(ObjectId id)
        {
            var results = Db.Find<T>(Query<T>.EQ(p => p.Id, id));
            return results.SingleOrDefault();
        }
    }
}