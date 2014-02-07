using System;
using NUnit.Framework;
using ServiceStack.Text;

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

            MongoCollection.Insert(new CachedItem(_user.Id.ToString(), _user, DateTime.Now.AddSeconds(60)));
        }

        [TearDown]
        public void TearDown()
        {
            MongoCollection.RemoveAll();
        }

        [Test]
        public void retrieves_item_from_cache()
        {
            var instance = MongoDbApiOutputCache.Get(_user.Id.ToString()) as JsonObject;

            //not sure this is correct
            //http://stackoverflow.com/a/21603948/131809

            Assert.That(instance.Get<Guid>("Id"), Is.EqualTo(_user.Id));
            Assert.That(instance.Get<string>("Name"), Is.EqualTo(_user.Name));
            Assert.That(instance.Get<DateTime>("DateOfBirth").Day, Is.EqualTo(_user.DateOfBirth.Day));
            Assert.That(instance.Get<DateTime>("DateOfBirth").Month, Is.EqualTo(_user.DateOfBirth.Month));
            Assert.That(instance.Get<DateTime>("DateOfBirth").Year, Is.EqualTo(_user.DateOfBirth.Year));
        }
    }
}