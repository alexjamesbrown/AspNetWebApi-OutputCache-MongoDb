using MongoDB.Driver;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    public class MongoDbApiOutputCacheTestsBase
    {
        protected MongoDatabase MongoDatabase;
        protected MongoCollection MongoCollection;

        protected MongoDbApiOutputCache MongoDbApiOutputCache;

        public MongoDbApiOutputCacheTestsBase()
        {
            var mongoUrl = new MongoUrl("mongodb://localhost/MongoDbApiOutputCache_Test");
            var client = new MongoClient(mongoUrl);
            var server = client.GetServer();

            MongoDatabase = server.GetDatabase(mongoUrl.DatabaseName);
            MongoCollection = MongoDatabase.GetCollection("cache");

            MongoDbApiOutputCache = new MongoDbApiOutputCache(MongoDatabase);
        }
    }
}