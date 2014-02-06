using System;

namespace WebAPI.OutputCache.MongoDb.Tests.Methods
{
    public class UserFixture
    {
        public UserFixture()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}