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

        public CachedItem(string key, object value)
        {
            Key = key;
            Value = JsonSerializer.SerializeToString(value);
        }

        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }
}