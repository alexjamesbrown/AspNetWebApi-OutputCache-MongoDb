using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ServiceStack.Text;
using WebApi.OutputCache.Core.Cache;

namespace WebAPI.OutputCache.MongoDb
{
    public class MongoDbApiOutputCache : IApiOutputCache
    {
        internal readonly MongoCollection MongoCollection;

        public MongoDbApiOutputCache(MongoDatabase mongoDatabase)
            : this(mongoDatabase, "cache")
        { }

        public MongoDbApiOutputCache(MongoDatabase mongoDatabase, string cache)
        {
            MongoCollection = mongoDatabase.GetCollection(cache);
        }

        public void RemoveStartsWith(string key)
        {
            MongoCollection.Remove(Query.Matches("_id", new BsonRegularExpression("^" + key)));
        }

        public T Get<T>(string key) where T : class
        {
            var item = MongoCollection
                .FindOneAs<CachedItem>(Query.EQ("_id", new BsonString(key)));

            return CheckItemExpired(item)
                ? null
                : JsonSerializer.DeserializeFromString<T>(item.Value);
        }

        private static bool CheckItemExpired(CachedItem item)
        {
            return item.Expiration < DateTime.Now;
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
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
            var cachedItem = new CachedItem(key, o, expiration);

            MongoCollection.Save(cachedItem);
        }
    }
}