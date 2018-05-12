using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi.OutputCache.Core.Cache;
using JsonSerializer = ServiceStack.Text.JsonSerializer;

namespace WebAPI.OutputCache.MongoDb
{
    public class MongoDbApiOutputCache : IApiOutputCache
    {
        internal readonly IMongoCollection<CachedItem> MongoCollection;

        public MongoDbApiOutputCache(IMongoDatabase mongoDatabase)
            : this(mongoDatabase, "cache")
        { }

        static MongoDbApiOutputCache()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(CachedItem)))
                BsonClassMap.RegisterClassMap<CachedItem>(cm =>
                {
                    cm.MapIdField(x => x.Key);
                    cm.MapProperty(x => x.Value).SetElementName("value");
                    cm.MapProperty(x => x.ExpireAt).SetElementName("expireAt");
                    cm.MapField(x => x.ValueType).SetElementName("valueType");

                    cm.SetIgnoreExtraElements(true);
                });
        }

        public MongoDbApiOutputCache(IMongoDatabase mongoDatabase, string cacheCollectionName)
        {
            MongoCollection = mongoDatabase.GetCollection<CachedItem>(cacheCollectionName);

            MongoCollection.Indexes.CreateOne(
                Builders<CachedItem>.IndexKeys.Ascending(x => x.ExpireAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromMilliseconds(0) }
            );
        }

        public IEnumerable<string> AllKeys
            => MongoCollection.AsQueryable().Select(x => x.Key);

        public void RemoveStartsWith(string key)
        {
            MongoCollection
                .DeleteMany(Builders<CachedItem>
                    .Filter.Regex("_id", new BsonRegularExpression($"^{key}")));
        }

        public T Get<T>(string key) where T : class
        {
            var item = MongoCollection
                .Find(x => x.Key.Equals(key))
                .FirstOrDefault();

            if (item == null)
                return null;

            return CheckItemExpired(item)
                ? null : JsonSerializer.DeserializeFromString<T>(item.Value);
        }

        public object Get(string key)
        {
            var item = MongoCollection
                .Find(x => x.Key.Equals(key))
                .FirstOrDefault();

            var type = Type.GetType(item.ValueType);
            return JsonSerializer.DeserializeFromString(item.Value, type);
        }

        public void Remove(string key)
        {
            MongoCollection
                .DeleteOne(x => x.Key.Equals(key));
        }

        public bool Contains(string key)
        {
            return MongoCollection
                .Find(x => x.Key.Equals(key))
                .Count() > 0;
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            if (key.Length > 256) //saves calling getByteCount if we know it could be less than 1024 bytes
                if (Encoding.UTF8.GetByteCount(key) >= 1024)
                    throw new KeyTooLongException();

            var cachedItem = new CachedItem(key, o, expiration.DateTime);

            MongoCollection.ReplaceOne(
                Builders<CachedItem>.Filter.Eq(x => x.Key, cachedItem.Key),
                cachedItem,
                new UpdateOptions { IsUpsert = true });
        }

        private bool CheckItemExpired(CachedItem item)
        {
            if (item.ExpireAt.ToLocalTime() >= DateTime.Now)
                return false;

            //does the work of TTL collections early - TTL is only "fired" once a minute or so
            MongoCollection.DeleteOne(x => x.Key.Equals(item.Key));

            return true;
        }
    }
}