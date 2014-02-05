using MongoDB.Bson.Serialization.Attributes;

namespace WebAPI.OutputCache.MongoDb
{
    [BsonIgnoreExtraElements]
    public class CachedItem
    {
        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("value")]
        public object Value { get; set; }
    }
}