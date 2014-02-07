using System;
using System.Threading;
using MongoDB.Driver.Builders;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    [TestFixture]
    public class MongoDbApiOutputCacheTests : MongoDbApiOutputCacheTestsBase
    {
        [Test]
        public void collection_name_is_cache_by_default()
        {
            var mongoDbApiOutputCache = new MongoDbApiOutputCache(MongoDatabase);
            Assert.That(mongoDbApiOutputCache.MongoCollection.Name, Is.EqualTo("cache"));
        }

        [Test]
        public void can_specify_collection_name()
        {
            var mongoDbApiOutputCache = new MongoDbApiOutputCache(MongoDatabase, "myCache");
            Assert.That(mongoDbApiOutputCache.MongoCollection.Name, Is.EqualTo("myCache"));
        }

        //due to the nature of the way mongodb ttl collections work this test takes over a minute to run
        //we must sleep the thread for over a minute, as the process that removes expired documents runs once a minute or so!
        [Test]
        public void items_are_deleted_from_database_automatically_when_expired()
        {
            var mongoDbApiOutputCache = new MongoDbApiOutputCache(MongoDatabase);

            mongoDbApiOutputCache.Add("something", "a value", DateTime.Now.AddMilliseconds(250));

            //wait just over a minute....
            Thread.Sleep(61000);

            //get the thing directly from mongo
            var resultDirectlyFromMongo = MongoCollection.FindOneAs<CachedItem>(Query.EQ("_id", "something"));

            Assert.That(resultDirectlyFromMongo, Is.Null);
        }
    }
}