using System;
using System.IO;
using Murmur;

namespace WebAPI.OutputCache.MongoDb.Utilities
{
    public class CalculateKey
    {
        public string Calculate(string key, string prefix = null)
        {
            if (prefix == null)
                prefix = string.Empty;

            if ((key.Length + prefix.Length) < 130)
                return key;

            using (var murmur128 = MurmurHash.Create128(managed: false))
                return string.Concat(prefix, Convert.ToBase64String(murmur128.ComputeHash(GenerateStreamFromString(key))));
        }

        internal static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}