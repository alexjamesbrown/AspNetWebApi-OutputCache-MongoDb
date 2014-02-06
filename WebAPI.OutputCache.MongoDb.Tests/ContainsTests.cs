using System;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    [TestFixture]
    public class ContainsTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user1;
        private UserFixture _user2;

        [SetUp]
        public void SetUp()
        {
            _user1 = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };
            _user2 = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };

            MongoCollection.Insert(new CachedItem(_user1.Id.ToString(), _user1));
            MongoCollection.Insert(new CachedItem(_user2.Id.ToString(), _user2));
        }

        [TearDown]
        public void TearDown()
        {
            MongoCollection.RemoveAll();
        }

        [Test]
        public void returns_true_if_item_is_in_collection()
        {
            var result = MongoDbApiOutputCache.Contains(_user1.Id.ToString());

            Assert.That(result, Is.True);
        }

        [Test]
        public void returns_false_if_item_is_not_in_collection()
        {
            var result = MongoDbApiOutputCache.Contains("i know this won't be there");

            Assert.That(result, Is.False);
        }
    }
}