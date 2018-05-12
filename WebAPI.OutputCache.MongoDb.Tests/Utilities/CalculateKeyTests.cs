using Murmur;
using NUnit.Framework;
using System;
using System.Security.Cryptography;
using WebAPI.OutputCache.MongoDb.Utilities;

namespace WebAPI.OutputCache.MongoDb.Tests.Utilities
{
    [TestFixture]
    public class CalculateKeyTests
    {
        private CalculateKey _calculateKey;

        [SetUp]
        public void SetUp()
        {
            _calculateKey = new CalculateKey();
        }

        [Test]
        public void Returns_plain_text_key_if_key_length_is_less_than_130()
        {
            //according to this http://stackoverflow.com/a/9533324/131809
            //each char could be up to 6 bytes

            var key = "";

            for (var i = 0; i < 3; i++)
                key += Guid.NewGuid().ToString();

            var result = _calculateKey.Calculate(key);

            Assert.That(result, Is.EqualTo(key));
        }

        [Test]
        public void Returns_hashed_key_if_passed_in_string_is_greater_than_130()
        {
            var key = "";

            for (var i = 0; i < 10000; i++)
                key += Guid.NewGuid().ToString();

            var result = _calculateKey.Calculate(key);

            HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
            var expectedKey = Convert.ToBase64String(murmur128.ComputeHash(CalculateKey.GenerateStreamFromString(key)));

            Assert.That(result, Is.EqualTo(expectedKey));
        }

        [Test]
        public void Calculates_different_key_for_different_values()
        {
            var key1 = "";
            var key2 = "";

            for (var i = 0; i < 100; i++)
                key1 += Guid.NewGuid().ToString();

            for (var i = 0; i < 100; i++)
                key2 += Guid.NewGuid().ToString();

            var result1 = _calculateKey.Calculate(key1);
            var result2 = _calculateKey.Calculate(key2);

            Assert.That(result1, Is.Not.EqualTo(result2));
        }

        [Test]
        public void Does_not_include_specified_prefix_in_hashed_key()
        {
            var key = "";

            for (var i = 0; i < 100; i++)
                key += Guid.NewGuid().ToString();

            var result = _calculateKey.Calculate(key, "someprefix");

            Assert.That(result, Does.StartWith("someprefix"));
        }
    }
}