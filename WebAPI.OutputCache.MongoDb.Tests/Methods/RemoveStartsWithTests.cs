using System;
using System.Linq;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    [TestFixture]
    public class RemoveStartsWithTests : MongoDbApiOutputCacheTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            var item1 = new CachedItem("apples-1", "Golden Delicious", DateTime.Now.AddHours(1));
            var item2 = new CachedItem("apples-2", "Pink Lady", DateTime.Now.AddHours(1));
            var item3 = new CachedItem("dogs-1", "Jack Russell", DateTime.Now.AddHours(1));

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