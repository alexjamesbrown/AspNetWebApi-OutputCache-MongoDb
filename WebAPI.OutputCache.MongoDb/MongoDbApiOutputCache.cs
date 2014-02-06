using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using WebApi.OutputCache.Core.Cache;
using JsonSerializer = ServiceStack.Text.JsonSerializer;

namespace WebAPI.OutputCache.MongoDb
{
    public class MongoDbApiOutputCache : IApiOutputCache
    {
        internal readonly MongoCollection MongoCollection;

        public MongoDbApiOutputCache(MongoDatabase mongoDatabase)
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

                    cm.SetIgnoreExtraElements(true);
                });
        }

        public MongoDbApiOutputCache(MongoDatabase mongoDatabase, string cacheCollectionName)
        {
            MongoCollection = mongoDatabase.GetCollection(cacheCollectionName);

            MongoCollection.EnsureIndex(
                IndexKeys.Ascending("expireAt"),
                IndexOptions.SetTimeToLive(TimeSpan.FromMilliseconds(0))
                );
        }

        public void RemoveStartsWith(string key)
        {
            MongoCollection.Remove(Query.Matches("_id", new BsonRegularExpression("^" + key)));
        }

        public T Get<T>(string key) where T : class
        {
            var item = MongoCollection
                .FindOneAs<CachedItem>(Query.EQ("_id", new BsonString(key)));

            if (item == null)
                return null;

            return CheckItemExpired(item)
                ? null
                : JsonSerializer.DeserializeFromString<T>(item.Value);
        }

        public object Get(string key)
        {
            var item = MongoCollection
                .FindOneAs<CachedItem>(Query.EQ("key", new BsonString(key)));

            return JsonSerializer.DeserializeFromString<object>(item.Value);
        }

        public void Remove(string key)
        {
            MongoCollection.Remove(Query.EQ("_id", new BsonString(key)));
        }

        public bool Contains(string key)
        {
            return MongoCollection
                .FindAs<CachedItem>(Query.EQ("_id", new BsonString(key)))
                .Count() == 1;
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            var cachedItem = new CachedItem(key, o, expiration.DateTime);

            MongoCollection.Save(cachedItem);
        }

        private bool CheckItemExpired(CachedItem item)
        {
            if (item.ExpireAt >= DateTime.Now)
                return false;

            //does the work of TTL collections early - TTL is only "fired" once a minute or so
            MongoCollection.Remove(Query.EQ("_id", item.Key));

            return true;
        }
    }
}