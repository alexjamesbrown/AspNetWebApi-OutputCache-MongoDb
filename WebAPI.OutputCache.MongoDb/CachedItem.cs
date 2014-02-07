using System;
using ServiceStack.Text;

namespace WebAPI.OutputCache.MongoDb
{
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

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTimeOffset Expiration { get; set; }
    }
}