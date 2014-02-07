using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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