﻿using System;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    [TestFixture]
    public class GetGenericTests : MongoDbApiOutputCacheTestsBase
    {
        private UserFixture _user;

        [SetUp]
        public void SetUp()
        {
            _user = new UserFixture { Name = "John", DateOfBirth = new DateTime(1980, 01, 23) };

            MongoCollection.InsertOne(new CachedItem(_user.Id.ToString(), _user, DateTime.Now.AddHours(1)));
        }

        [TearDown]
        public void TearDown()
        {
            MongoDatabase.DropCollection(MongoCollection.CollectionNamespace.CollectionName);
        }

        [Test]
        public void Retrieves_item_from_cache()
        {
            var result = MongoDbApiOutputCache.Get<UserFixture>(_user.Id.ToString());

            Assert.That(result, Is.InstanceOf<UserFixture>());
            Assert.That(result.Id, Is.EqualTo(_user.Id));
            Assert.That(result.Name, Is.EqualTo(_user.Name));
            Assert.That(result.DateOfBirth.Day, Is.EqualTo(_user.DateOfBirth.Day));
            Assert.That(result.DateOfBirth.Month, Is.EqualTo(_user.DateOfBirth.Month));
            Assert.That(result.DateOfBirth.Year, Is.EqualTo(_user.DateOfBirth.Year));
        }

        [Test]
        public void Returns_null_if_item_not_in_collection()
        {
            var result = MongoDbApiOutputCache.Get<UserFixture>("unknown key");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Does_not_return_item_that_has_expired()
        {
            //add an item that expires 1 hour ago
            MongoCollection.InsertOne(new CachedItem("expired-item", _user, DateTime.Now.AddHours(-1)));

            var result = MongoDbApiOutputCache.Get<UserFixture>("expired-item");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Item_is_deleted_from_database_if_expired()
        {
            //add an item that expires 1 hour ago
            MongoCollection.InsertOne(new CachedItem("expired-item", _user, DateTime.Now.AddHours(-1)));

            var result = MongoDbApiOutputCache.Get<UserFixture>("expired-item");
            var resultFromMongo = MongoCollection
                .Find(x => x.Key.Equals("expired-item"))
                .FirstOrDefault();

            Assert.That(result, Is.Null);
            Assert.That(resultFromMongo, Is.Null);
        }
    }
}