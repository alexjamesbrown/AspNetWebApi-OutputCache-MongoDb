using System;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.Text;

namespace WebAPI.OutputCache.MongoDb
{
    public class CachedItem
    {
        public CachedItem(string key, object value, DateTime expireAt)
        {
            Key = key;
            Value = JsonSerializer.SerializeToString(value);
            ExpireAt = expireAt;
        }

        [BsonId]
        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime ExpireAt { get; set; }
    }
}