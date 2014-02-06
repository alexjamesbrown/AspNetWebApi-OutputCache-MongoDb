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
        private readonly MongoCollection _mongoCollection;

        public MongoDbApiOutputCache(MongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection("cache");
        }

        public void RemoveStartsWith(string key)
        {
            _mongoCollection.Remove(Query.Matches("key", new BsonRegularExpression("^" + key)));
        }

        public T Get<T>(string key) where T : class
        {
            var item = _mongoCollection
                .FindOneAs<CachedItem>(Query.EQ("key", new BsonString(key)));

            return JsonSerializer.DeserializeFromString<T>(item.Value);
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            _mongoCollection.Remove(Query.EQ("key", new BsonString(key)));
        }

        public bool Contains(string key)
        {
            return _mongoCollection
                .FindAs<CachedItem>(Query.EQ("key", new BsonString(key)))
                .Count() == 1;
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            var cachedItem = new CachedItem(key, o, expiration);

            _mongoCollection.Insert(cachedItem);
        }
    }
}