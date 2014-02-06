using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.Text;

namespace WebAPI.OutputCache.MongoDb
{
    [BsonIgnoreExtraElements]
    public class CachedItem
    {
        public CachedItem()
        {
        }

        public CachedItem(string key, object value, DateTimeOffset expiration)
        {
            Key = key;
            Value = JsonSerializer.SerializeToString(value);
            Expiration = expiration;
        }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }

        [BsonElement("expiration")]
        public DateTimeOffset Expiration { get; set; }
    }
}