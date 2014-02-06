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
    }
}