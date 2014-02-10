using System;
using System.IO;
using Murmur;

namespace WebAPI.OutputCache.MongoDb.Utilities
{
    /// <summary>
    /// Utility to calculate a key less than the 1024 byte maximum
    /// </summary>
    public class CalculateKey
    {
        /// <summary>
        ///Returns the original key if already less than 1024 bytes, or hashes, then returns
        /// </summary>
        /// <param name="key">The original key</param>
        /// <param name="prefix">Optional prefix - won't be hashed</param>
        /// <returns></returns>
        public string Calculate(string key, string prefix = null)
        {
            if (prefix == null)
                prefix = string.Empty;

            if (key.Length < 256)
                return key;

            using (var murmur128 = MurmurHash.Create128(managed: false))
                return string.Concat(prefix,
                    Convert.ToBase64String(murmur128.ComputeHash(GenerateStreamFromString(key))));
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