using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    public class MongoDbApiOutputCacheTestsBase
    {
        protected IMongoDatabase MongoDatabase;
        protected IMongoCollection<CachedItem> MongoCollection;

        protected MongoDbApiOutputCache MongoDbApiOutputCache;

        static MongoDbApiOutputCacheTestsBase()
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

        public MongoDbApiOutputCacheTestsBase()
        {
            var mongoUrl = new MongoUrl("mongodb://localhost:27017/MongoDbApiOutputCache_Test");
            var mongoClient = new MongoClient(mongoUrl);

            MongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            MongoCollection = MongoDatabase.GetCollection<CachedItem>("cache");

            MongoDbApiOutputCache = new MongoDbApiOutputCache(MongoDatabase);
        }
    }
}