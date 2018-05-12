using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ServiceStack.Text;
using System;
using System.Linq;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    [TestFixture]
    public class RemoveTests : MongoDbApiOutputCacheTestsBase
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
        public void Removes_item_from_cache()
        {
            MongoDbApiOutputCache.Remove(_user1.Id.ToString());

            Assert.That(MongoCollection.Count(new BsonDocument()), Is.EqualTo(1));

            var users = MongoCollection
                .AsQueryable()
                .Select(cachedItem => cachedItem.Value)
                .ToList();

            Assert.That(JsonSerializer.DeserializeFromString<UserFixture>(users.First()).Id, Is.Not.EqualTo(_user1.Id));
        }
    }
}