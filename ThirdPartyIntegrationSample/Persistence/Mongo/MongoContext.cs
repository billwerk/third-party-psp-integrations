using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Core.Helpers;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Exceptions;
using Persistence.Interfaces;

namespace Persistence.Mongo
{
    public class MongoContext : IMongoContext
    {
        public const string MongoDatabaseMain = "thirdparty";
        private readonly ILogger<MongoContext> _logger;

        static MongoContext()
        {
            RegisterBsonSerializers.EnsureRegistered();
        }

        public bool Exists<T>(ObjectId id) where T : IDatabaseObject => GetCollection<T>().Find(Query<T>.EQ(p => p.Id, id).ToBsonDocument()).Limit(1).CountDocuments() == 1;

        public void AssertIdExists<T>(ObjectId id)
            where T : IDatabaseObject
        {
            var temp = Exists<T>(id);
            if (temp == false)
                throw new ApplicationException($"Foreign key violation: Couldn't find {typeof(T).Name}:{id.ToString()}");
        }

        public MongoContext(IMongoClient mongoClient, ILogger<MongoContext> logger)
        {
            _logger = logger;
            Provider = mongoClient.GetDatabase(MongoDatabaseMain);
            CodeContract.Assert(Provider != null, "_mongoDatabase != null");
        }

        public IMongoDatabase Provider { get; }

        public T SingleById<T>(ObjectId id) where T : IDatabaseObject => SingleById<T, ObjectId>(id);

        public T SingleById<T, TId>(TId id)
            where T : IDatabaseObject<TId>
        {
            return GetCollection<T>()
                .Find(Builders<T>.Filter.Eq("_id", BsonValue.Create(id)))
                .SingleOrDefault();
        }

        public T SingleById<T>(string id) =>
        ObjectId.TryParse(id, out var objectid) ?
                GetCollection<T>().Find(Builders<T>.Filter.Eq("_id", BsonValue.Create(objectid))).SingleOrDefault() :
                GetCollection<T>().Find(Builders<T>.Filter.Eq("_id", BsonValue.Create(id))).SingleOrDefault();

        /// <summary>
        /// Inserts the given item of type T into a collection named after the type, more specifically,
        /// a collection with name typeof(T).Name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Insert<T>(T item) where T : class
        {
            try
            {
                GetCollection<T>().InsertOne(item);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public void InsertBatch<T1>(IEnumerable<T1> batch)
        {
            try
            {
                GetCollection<T1>().InsertMany(batch);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long UpdateMany<T>(IMongoCollection<T> collection, IMongoQuery query, IMongoUpdate update)
        {
            try
            {
                var result = collection.UpdateMany(query.ToBsonDocument(), update.ToBsonDocument());
                if (!result.IsAcknowledged)
                {
                    _logger.Log(LogLevel.Error, CreateLogMessage(nameof(UpdateMany), typeof(T).Name));
                    throw new ApplicationException(CreateAppExceptionMessage("update"));
                }

                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long UpdateMany<T>(IMongoQuery query, IMongoUpdate update)
        {
            return UpdateMany(GetCollection<T>(), query, update);
        }

        public long Update<T>(T item) where T : IDatabaseObject
        {
            return UpdateInternal(item, item.Id);
        }

        private long UpdateInternal<T>(T item, object id)
        {
            try
            {
                var result = GetCollection<T>().ReplaceOne(Query.EQ("_id", BsonValue.Create(id)).ToBsonDocument(), item);
                if (!result.IsAcknowledged)
                {
                    _logger.Log(LogLevel.Error, CreateLogMessage(nameof(UpdateMany), typeof(T).Name));
                    throw new ApplicationException(CreateAppExceptionMessage("update"));
                }

                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        /// <summary>
        /// Performs an update with a more strict write concern to ensure the update was persisted.
        /// Use this whenever data loss is irrecoverable, such as in webhooks. Do not use this method
        /// per default, because it can easily take 10 - 30ms to complete, rendering it an order of
        /// magnitude slower than normal updates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="newItemState"></param>
        /// <returns></returns>
        public long UpdateSafe<T>(IMongoQuery query, T newItemState) => UpdateReplaceInternal(query, newItemState);
        public long Update<T>(IMongoQuery query, T newItemState) => UpdateReplaceInternal(query, newItemState);

        private long UpdateReplaceInternal<T>(IMongoQuery query, T newItemState)
        {
            try
            {
                var result = GetCollection<T>().ReplaceOne(query.ToBsonDocument(), newItemState, new ReplaceOptions { IsUpsert = false });
                if (!result.IsAcknowledged)
                {
                    _logger.Log(LogLevel.Error, CreateLogMessage(nameof(UpdateReplaceInternal), typeof(T).Name));
                    throw new ApplicationException(CreateAppExceptionMessage("UpdateReplaceInternal"));
                }

                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long Update<T>(IMongoQuery query, IMongoUpdate update) => UpdateDiffInternal<T>(query, update);
        public long UpdateSafe<T>(IMongoQuery query, IMongoUpdate update) => UpdateDiffInternal<T>(query, update);

        private long UpdateDiffInternal<T>(IMongoQuery query, IMongoUpdate update)
        {
            try
            {
                var result = GetCollection<T>().UpdateOne(query.ToBsonDocument(), update.ToBsonDocument());// , new UpdateOptions {   writeConcern);
                if (!result.IsAcknowledged)
                {
                    _logger.Log(LogLevel.Error, CreateLogMessage(nameof(UpdateDiffInternal), typeof(T).Name));
                    throw new ApplicationException(CreateAppExceptionMessage("UpdateDiffInternal"));
                }

                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long Upsert<T>(IMongoQuery query, IMongoUpdate update)
        {
            CodeContract.Requires<ArgumentNullException>(query != null, "query != null");
            CodeContract.Requires<ArgumentNullException>(update != null, "update != null");

            try
            {
                var result = GetCollection<T>().UpdateOne(query.ToBsonDocument(), update.ToBsonDocument(), new UpdateOptions { IsUpsert = true });
                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long Replace<T>(FilterDefinition<T> filter, T item)
        {
            try
            {
                // Upsert functionality is added

                var result = GetCollection<T>().ReplaceOne(filter, item, new ReplaceOptions { IsUpsert = true });
                if (!result.IsAcknowledged)
                {
                    _logger.Log(LogLevel.Error, CreateLogMessage(nameof(UpdateMany), typeof(T).Name));
                    throw new ApplicationException(CreateAppExceptionMessage("replace"));
                }

                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public long Upsert<T>(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            CodeContract.Requires<ArgumentNullException>(filter != null, "filter != null");
            CodeContract.Requires<ArgumentNullException>(update != null, "update != null");

            try
            {
                var result = GetCollection<T>().UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
                return Math.Max(result.MatchedCount, result.ModifiedCount);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public T FindModifyUpsert<T>(IMongoQuery query, IMongoUpdate update, WriteConcern writeConcern)
        {
            CodeContract.Requires<ArgumentNullException>(query != null, "query != null");
            CodeContract.Requires<ArgumentNullException>(update != null, "update != null");

            try
            {
                var options = new FindOneAndUpdateOptions<T>
                {
                    Sort = null,
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                return GetCollection<T>(writeConcern).FindOneAndUpdate(query.ToBsonDocument(), update.ToBsonDocument(), options);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public T FindModifyUpsert<T>(IMongoQuery query, IMongoUpdate update)
        {
            CodeContract.Requires<ArgumentNullException>(query != null, "query != null");
            CodeContract.Requires<ArgumentNullException>(update != null, "update != null");

            try
            {
                var options = new FindOneAndUpdateOptions<T>
                {
                    Sort = null,
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

                return GetCollection<T>().FindOneAndUpdate(query.ToBsonDocument(), update.ToBsonDocument(), options);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        public T FindAndModifyOne<T>(IMongoQuery query, IMongoUpdate update, bool isUpsert = false) where T : class
        {
            try
            {
                var options = new FindOneAndUpdateOptions<T>
                {
                    Sort = null,
                    IsUpsert = isUpsert,
                    ReturnDocument = ReturnDocument.After
                };

                return GetCollection<T>().FindOneAndUpdate(query.ToBsonDocument(), update.ToBsonDocument(), options);
            }
            catch (MongoException ex)
            {
                UniqueConstraintViolationException.CheckAndRaise(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes a single item from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Delete<T>(T item) where T : IDatabaseObject
        {
            CodeContract.Requires(item.Id != ObjectId.Empty, "item.Id != ObjectId.Empty");

            //var query = QueryWrapper.Create(item);
            var query = Query.EQ("_id", BsonValue.Create(item.Id));
            var result = GetCollection<T>().DeleteOne(query.ToBsonDocument());

            if (!result.IsAcknowledged)
            {
                _logger.Log(LogLevel.Error, CreateLogMessage(nameof(Delete), typeof(T).Name));
                throw new ApplicationException(CreateAppExceptionMessage("delete"));
            }

            if (result.DeletedCount != 1)
            {
                _logger.Log(LogLevel.Error, CreateLogMessage(nameof(Delete), typeof(T).Name));
                throw new ApplicationException("delete2");
            }

        }

        /// <summary>
        /// Deletes a single item from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns>The number of affected documents</returns>
        public long Delete<T>(IMongoQuery query)
        {
            var result = GetCollection<T>().DeleteOne(query.ToBsonDocument());

            if (!result.IsAcknowledged)
            {
                _logger.Log(LogLevel.Error, CreateLogMessage(nameof(Delete), typeof(T).Name));
                throw new ApplicationException(CreateAppExceptionMessage("delete"));
            }

            return result.DeletedCount;
        }

        public long DeleteMany<T>(IMongoQuery query)
            where T : class
        {
            var result = GetCollection<T>().DeleteMany(query.ToBsonDocument());
            if (!result.IsAcknowledged)
            {
                _logger.Log(LogLevel.Error, CreateLogMessage(nameof(DeleteMany), typeof(T).Name));
                throw new ApplicationException(CreateAppExceptionMessage("delete"));
            }

            return result.DeletedCount;
        }

        public bool TryInsert<T>(T item) where T : class
        {
            try
            {
                Insert(item);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warning, "Error in TryInsert!", ex);
                return false;
            }
            return true;
        }

        public IMongoCollection<T1> GetCollection<T1>(string collectionName)
        {
            var result = Provider.GetCollection<T1>(collectionName);
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IMongoCollection<T1> GetCollection<T1>()
        {
            var result = Provider.GetCollection<T1>(typeof(T1).Name);
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IMongoCollection<T1> GetCollection<T1>(WriteConcern writeConcern)
        {
            var result = Provider.GetCollection<T1>(typeof(T1).Name, new MongoCollectionSettings { WriteConcern = writeConcern });
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IFindFluent<T1, T1> FindAll<T1>()
        {
            var result = GetCollection<T1>().Find(_ => true);
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IFindFluent<T1, T1> Find<T1>(IMongoQuery query, FindOptions options = null)
        {
            var result = GetCollection<T1>().Find(query.ToBsonDocument(), options);
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IFindFluent<T1, T1> Find<T1>(FilterDefinition<T1> query, FindOptions options = null)
        {
            var result = GetCollection<T1>().Find(query, options);
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        public IEnumerable<T1> Find<T1>(Expression<Func<T1, bool>> filter, FindOptions options = null)
        {
            var result = GetCollection<T1>().Find(Query<T1>.Where(filter).ToBsonDocument(), options).ToEnumerable();
            CodeContract.Assert(result != null, "result != null");
            return result;
        }

        private static string CreateLogMessage(string methodName, string typeName) => $"Failed to perform DB operation {methodName} on object of type {typeName}";
        private static string CreateAppExceptionMessage(string operation) => $"Database {operation} operation failed";
    }
}
