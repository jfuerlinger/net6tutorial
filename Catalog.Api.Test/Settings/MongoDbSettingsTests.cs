using Catalog.Api.Settings;
using Xunit;

namespace Catalog.Api.Test.Settings
{
    public class MongoDbSettingsTests
    {
        [Fact]
        public void GetConnectionString_CallGetter_ShouldDeliverCorrectConnectionString()
        {
            // Arrange
            var mongoDbSettings = new MongoDbSettings
            {
                Host = "localhost",
                Port = 80,
                User = "user1",
                Password = "password"
            };

            // Act
            var connectionString = mongoDbSettings.ConnectionString;

            // Assert
            Assert.NotNull(connectionString);
            Assert.Equal("mongodb://user1:password@localhost:80", connectionString);
        }
    }
}
