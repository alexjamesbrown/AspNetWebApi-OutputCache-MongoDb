using System;

namespace WebAPI.OutputCache.MongoDb
{
    public class KeyTooLongException : ArgumentException
    {
        public KeyTooLongException()
            : base("The key provided was over the 1024 bytes maximum for an indexed MongoDb field")
        { }
    }
}