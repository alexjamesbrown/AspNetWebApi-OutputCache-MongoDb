using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using ServiceStack.Text;
using System;
using System.Linq;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    [TestFixture]
    public class AddTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user;

        [SetUp]
        public void SetUp()
        {
            MongoDatabase.DropCollection(MongoCollection.CollectionNamespace.CollectionName);

            _user = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };
        }

        [Test]
        public void Adds_item_to_cache()
        {
            MongoDbApiOutputCache.Add(_user.Id.ToString(), _user, DateTime.Now.AddSeconds(60));

            var item = MongoCollection.AsQueryable().FirstOrDefault();

            Assert.That(item, Is.Not.Null);
        }

        [Test]
        public void Added_item_stored_with_supplied_key()
        {
            var key = _user.Id.ToString();
            MongoDbApiOutputCache.Add(key, _user, DateTime.Now.AddSeconds(60));

            var item = MongoCollection.AsQueryable().FirstOrDefault();

            Assert.That(item.Key, Is.EqualTo(key));
        }

        [Test]
        public void Added_item_stored_with_expiry()
        {
            var expiration = DateTime.Now.AddSeconds(60);

            MongoDbApiOutputCache.Add(_user.Id.ToString(), _user, expiration);

            //var item = MongoCollection.FindOneAs<CachedItem>();
            var item = MongoCollection.AsQueryable().FirstOrDefault();
            var itemExpireAt = item.ExpireAt.ToLocalTime();

            //todo: would be good to check they are the same value.. without this rubbish!
            //something like:
            //Assert.That(DateTime.Compare(item.ExpireAt, new DateTime(expiration.Ticks)), Is.EqualTo(0));

            //note: will this work?
            //Assert.That(itemExpireAt, Is.EqualTo(expiration).Within(1).Milliseconds);

            Assert.That(expiration.Day, Is.EqualTo(itemExpireAt.Day));
            Assert.That(expiration.Month, Is.EqualTo(itemExpireAt.Month));
            Assert.That(expiration.Year, Is.EqualTo(itemExpireAt.Year));
            Assert.That(expiration.Hour, Is.EqualTo(itemExpireAt.Hour));
            Assert.That(expiration.Minute, Is.EqualTo(itemExpireAt.Minute));
            Assert.That(expiration.Second, Is.EqualTo(itemExpireAt.Second));
            Assert.That(expiration.Millisecond, Is.EqualTo(itemExpireAt.Millisecond));
        }

        [Test]
        public void Adding_item_with_duplicate_key_updates_original()
        {
            MongoDbApiOutputCache.Add("user", _user, DateTime.Now.AddSeconds(60));

            var differentUser = new UserFixture { Name = "Simon" };

            MongoDbApiOutputCache.Add("user", differentUser, DateTime.Now.AddSeconds(60));

            Assert.That(MongoCollection.Count(new BsonDocument()), Is.EqualTo(1));

            var item = JsonSerializer.DeserializeFromString<UserFixture>(MongoCollection.AsQueryable().FirstOrDefault().Value);

            Assert.That(item, Is.Not.Null);
            Assert.That(item.Id, Is.EqualTo(differentUser.Id));
            Assert.That(item.Name, Is.EqualTo(differentUser.Name));
        }

        [Test]
        public void Adding_item_with_long_key_throws_exception()
        {
            var key = "";

            for (var i = 0; i < 100; i++)
                key += Guid.NewGuid().ToString();

            var exception = Assert.Throws<KeyTooLongException>(() => MongoDbApiOutputCache.Add(key, _user, DateTime.Now.AddSeconds(60)));

            Assert.That(exception.Message, Is.EqualTo("The key provided was over the 1024 bytes maximum for an indexed MongoDb field"));
        }
    }
}