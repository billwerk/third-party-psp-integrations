using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence.Interfaces;

namespace Persistence.Mongo
{
    public interface IMongoContext
    {
        IMongoDatabase Provider { get; }

        T SingleById<T>(ObjectId id) where T : IDatabaseObject;
        T SingleById<T, TId>(TId id) where T : IDatabaseObject<TId>;
        T SingleById<T>(string id);
        IFindFluent<T1, T1> FindAll<T1>();
        IFindFluent<T1, T1> Find<T1>(IMongoQuery query, FindOptions options = null);
        IFindFluent<T1, T1> Find<T1>(FilterDefinition<T1> query, FindOptions options = null);
        IMongoCollection<T1> GetCollection<T1>(string collectionName);
        IMongoCollection<T1> GetCollection<T1>();
        IMongoCollection<T1> GetCollection<T1>(WriteConcern writeConcern);

        long Update<T>(T item) where T : IDatabaseObject;
        long Update<T>(IMongoQuery query, T newItemState);
        long UpdateSafe<T>(IMongoQuery query, T newItemState);
        long Update<T>(IMongoQuery query, IMongoUpdate update);
        long UpdateSafe<T>(IMongoQuery query, IMongoUpdate update);
        long UpdateMany<T>(IMongoQuery query, IMongoUpdate update);
        long UpdateMany<T>(IMongoCollection<T> collection, IMongoQuery query, IMongoUpdate update);
        long Upsert<T>(IMongoQuery query, IMongoUpdate update);
        T FindAndModifyOne<T>(IMongoQuery query, IMongoUpdate update, bool isUpsert = false) where T : class;
        T FindModifyUpsert<T>(IMongoQuery query, IMongoUpdate update, WriteConcern writeConcern);
        T FindModifyUpsert<T>(IMongoQuery query, IMongoUpdate update);
        bool TryInsert<T>(T item) where T : class;
        void Insert<T>(T item) where T : class;
        void InsertBatch<T1>(IEnumerable<T1> batch);
        bool Exists<T>(ObjectId id) where T : IDatabaseObject;
       void AssertIdExists<T>(ObjectId id) where T : IDatabaseObject;

        long Replace<T>(FilterDefinition<T> filter, T item);
        void Delete<T>(T item) where T : IDatabaseObject;
        long Delete<T>(IMongoQuery query);
        long DeleteMany<T>(IMongoQuery query) where T : class;

    }
}