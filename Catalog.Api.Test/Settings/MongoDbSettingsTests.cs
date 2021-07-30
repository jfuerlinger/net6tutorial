using Catalog.Api.Settings;
using Xunit;

namespace Catalog.Api.Test.Settings
{
    public class MongoDbSettingsTests
    {
        [Theory]
        [AutoDomainData]
        public void GetConnectionString_CallGetter_ShouldDeliverCorrectConnectionString(
            MongoDbSettings mongoDbSettings)
        {
            // Arrange
            // Act
            var connectionString = mongoDbSettings.ConnectionString;

            // Assert
            Assert.NotNull(connectionString);
            Assert.Equal($"mongodb://{mongoDbSettings.User}:{mongoDbSettings.Password}@{mongoDbSettings.Host}:{mongoDbSettings.Port}", connectionString);
        }
    }
}
