using System.Linq;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    [TestFixture]
    public class RemoveStartsWithTests : MongoDbApiOutputCacheTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            var item1 = new CachedItem { Key = "apples-1", Value = "Golden Delicious" };
            var item2 = new CachedItem { Key = "apples-2", Value = "Pink Lady" };
            var item3 = new CachedItem { Key = "dogs-1", Value = "Jack Russell" };

            MongoCollection.Insert(item1);
            MongoCollection.Insert(item2);
            MongoCollection.Insert(item3);
        }

        [TearDown]
        public void TearDown()
        {
            MongoCollection.RemoveAll();
        }

        [Test]
        public void removes_keys_starting_with_given_string()
        {
            Assert.That(MongoCollection.Count(), Is.EqualTo(3));

            MongoDbApiOutputCache.RemoveStartsWith("apples");

            Assert.That(MongoCollection.Count(), Is.EqualTo(1));

            var result = MongoCollection.FindAllAs<CachedItem>();

            Assert.That(result.Any(x => x.Key.Equals("apples-1")), Is.False);
            Assert.That(result.Any(x => x.Key.Equals("apples-2")), Is.False);
        }
    }
}