using NUnit.Framework;
using System;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
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

            MongoCollection.InsertOne(new CachedItem(_user1.Id.ToString(), _user1, DateTime.Now.AddHours(1)));
            MongoCollection.InsertOne(new CachedItem(_user2.Id.ToString(), _user2, DateTime.Now.AddHours(1)));
        }

        [TearDown]
        public void TearDown()
        {
            MongoDatabase.DropCollection(MongoCollection.CollectionNamespace.CollectionName);
        }

        [Test]
        public void Returns_true_if_item_is_in_collection()
        {
            var result = MongoDbApiOutputCache.Contains(_user1.Id.ToString());

            Assert.That(result, Is.True);
        }

        [Test]
        public void Returns_false_if_item_is_not_in_collection()
        {
            var result = MongoDbApiOutputCache.Contains("i know this won't be there");

            Assert.That(result, Is.False);
        }
    }
}