using System;
using System.Linq;
using NUnit.Framework;
using ServiceStack.Text;

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

            MongoCollection.Insert(new CachedItem(_user1.Id.ToString(), _user1, DateTime.Now.AddHours(1)));
            MongoCollection.Insert(new CachedItem(_user2.Id.ToString(), _user2, DateTime.Now.AddHours(1)));
        }

        [TearDown]
        public void TearDown()
        {
            MongoCollection.RemoveAll();
        }

        [Test]
        public void removes_item_from_cache()
        {
            MongoDbApiOutputCache.Remove(_user1.Id.ToString());

            Assert.That(MongoCollection.Count(), Is.EqualTo(1));

            var users = MongoCollection
                .FindAllAs<CachedItem>()
                .Select(cachedItem => JsonSerializer.DeserializeFromString<UserFixture>(cachedItem.Value))
                .ToList();

            Assert.That(users.First().Id, Is.Not.EqualTo(_user1.Id));
        }
    }
}