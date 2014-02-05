using System;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using WebApi.OutputCache.Core.Cache;

namespace WebAPI.OutputCache.MongoDb
{
    public class MongoDbApiOutputCache : IApiOutputCache
    {
        private readonly MongoCollection _mongoCollection;

        public MongoDbApiOutputCache(MongoCollection mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public void RemoveStartsWith(string key)
        {
            _mongoCollection.Remove(Query.Matches("key", new BsonRegularExpression("^" + key)));
        }

        public T Get<T>(string key) where T : class
        {
            return _mongoCollection.FindOneAs<T>(Query.EQ("key", new BsonString(key)));
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            throw new NotImplementedException();
        }
    }
}