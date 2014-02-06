using System;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests
{
    [TestFixture]
    public class GetGenericTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user;

        [SetUp]
        public void SetUp()
        {
            _user = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };

            MongoCollection.Insert(new CachedItem(_user.Id.ToString(), _user));
        }

        [TearDown]
        public void TearDown()
        {
            MongoCollection.RemoveAll();
        }

        [Test]
        public void retrieves_item_from_cache()
        {
            var result = MongoDbApiOutputCache.Get<UserFixture>(_user.Id.ToString());

            Assert.That(result, Is.InstanceOf<UserFixture>());
            Assert.That(result.Id, Is.EqualTo(_user.Id));
            Assert.That(result.Name, Is.EqualTo(_user.Name));
            Assert.That(result.DateOfBirth.Day, Is.EqualTo(_user.DateOfBirth.Day));
            Assert.That(result.DateOfBirth.Month, Is.EqualTo(_user.DateOfBirth.Month));
            Assert.That(result.DateOfBirth.Year, Is.EqualTo(_user.DateOfBirth.Year));
        }
    }
}