using NUnit.Framework;
using System;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    [TestFixture]
    public class GetTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user;

        [SetUp]
        public void SetUp()
        {
            _user = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };

            MongoCollection.InsertOne(new CachedItem(_user.Id.ToString(), _user, DateTime.Now.AddSeconds(60)));
        }

        [TearDown]
        public void TearDown()
        {
            MongoDatabase.DropCollection(MongoCollection.CollectionNamespace.CollectionName);
        }

        [Test]
        public void Retrieves_item_from_cache()
        {
            var instance = MongoDbApiOutputCache.Get(_user.Id.ToString()) as UserFixture;

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.Id, Is.EqualTo(_user.Id));
            Assert.That(instance.Name, Is.EqualTo(_user.Name));
            Assert.That(instance.DateOfBirth.Day, Is.EqualTo(_user.DateOfBirth.Day));
            Assert.That(instance.DateOfBirth.Month, Is.EqualTo(_user.DateOfBirth.Month));
            Assert.That(instance.DateOfBirth.Year, Is.EqualTo(_user.DateOfBirth.Year));
        }
    }
}