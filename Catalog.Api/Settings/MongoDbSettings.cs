using System.Diagnostics.CodeAnalysis;

namespace Catalog.Api.Settings
{
    public class MongoDbSettings
    {
        [ExcludeFromCodeCoverage]
        public string Host { get; set; }
        [ExcludeFromCodeCoverage]
        public int Port { get; set; }
        [ExcludeFromCodeCoverage]
        public string User { get; set; }
        [ExcludeFromCodeCoverage]
        public string Password { get; set; }
        
        public string ConnectionString => $"mongodb://{User}:{Password}@{Host}:{Port}";
    }
}