using System;
using ServiceStack.Text;

namespace WebAPI.OutputCache.MongoDb
{
    public class CachedItem
    {
        public CachedItem(string key, object value, DateTime expireAt)
        {
            Key = key;
            Value = JsonSerializer.SerializeToString(value);
            ValueType = value.GetType().AssemblyQualifiedName;

            ExpireAt = expireAt;
        }

        public string Key { get; set; }

        public string Value { get; set; }
        public string ValueType { get; private set; }

        public DateTime ExpireAt { get; set; }
    }
}