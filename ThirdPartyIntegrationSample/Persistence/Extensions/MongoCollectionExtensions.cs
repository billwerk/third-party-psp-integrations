using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using MongoDB.Driver;

namespace Persistence.Extensions
{
    public static class MongoCollectionExtensions
    {
        public static IFindFluent<TDocument, TDocument> Find<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions options = null)
        {
            return session == null
                ? collection.Find(filter, options)
                : IMongoCollectionExtensions.Find(collection, session, filter, options);
        }

        public static IFindFluent<TDocument, TDocument> Find<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, Expression<Func<TDocument, bool>> filter, FindOptions options = null)
        {
            return session == null
                ? collection.Find(filter, options)
                : IMongoCollectionExtensions.Find(collection, session, filter, options);
        }

        public static void InsertOne<TDocument>(this IMongoCollection<TDocument> collection, IClientSessionHandle session,
            TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
        {
            if (session == null)
            {
                collection.InsertOne(document, options, cancellationToken);
                return;
            }

            collection.InsertOne(session, document, options, cancellationToken);
        }

        public static void InsertMany<TDocument>(this IMongoCollection<TDocument> collection, IClientSessionHandle session,
            IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
        {
            if (session == null)
            {
                collection.InsertMany(documents, options, cancellationToken);
                return;
            }

            collection.InsertMany(session, documents, options, cancellationToken);
        }

        public static UpdateResult UpdateOne<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update,
            UpdateOptions options = null, CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.UpdateOne(filter, update, options, cancellationToken)
                : collection.UpdateOne(session, filter, update, options, cancellationToken);
        }

        public static UpdateResult UpdateMany<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update,
            UpdateOptions options = null, CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.UpdateMany(filter, update, options, cancellationToken)
                : collection.UpdateMany(session, filter, update, options, cancellationToken);
        }

        public static TProjection FindOneAndUpdate<TProjection, TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update,
            FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.FindOneAndUpdate(filter, update, options, cancellationToken)
                : collection.FindOneAndUpdate(session, filter, update, options, cancellationToken);
        }

        public static ReplaceOneResult ReplaceOne<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement,
            ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.ReplaceOne(filter, replacement, options, cancellationToken)
                : collection.ReplaceOne(session, filter, replacement, options, cancellationToken);
        }

        public static DeleteResult DeleteOne<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.DeleteOne(filter, options, cancellationToken)
                : collection.DeleteOne(session, filter, options, cancellationToken);
        }

        public static DeleteResult DeleteMany<TDocument>(this IMongoCollection<TDocument> collection,
            IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null,
            CancellationToken cancellationToken = default)
        {
            return session == null
                ? collection.DeleteMany(filter, options, cancellationToken)
                : collection.DeleteMany(session, filter, options, cancellationToken);
        }
    }
}