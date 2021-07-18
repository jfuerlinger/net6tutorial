using Catalog.Api.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Test.Settings
{
    [TestClass]
    public class MongoDbSettingsTests
    {
        [TestMethod]
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
            Assert.IsNotNull(connectionString);
            Assert.AreEqual(connectionString, "mongodb://user1:password@localhost:80");
        }
    }
}
