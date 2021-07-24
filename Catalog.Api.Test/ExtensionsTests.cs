using Catalog.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Api.Test
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ExtensionsTests
    {
        [TestMethod]
        public void AsDto_Call_ShouldConvertToDto()
        {
            // Arrange
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = "Item 1",
                CreatedDate = DateTime.Now,
                Price = 15
            };

            // Act
            var dto = item.AsDto();

            // Assert
            Assert.IsTrue(condition: item.Id == dto.Id);
            Assert.IsTrue(condition: item.Name == dto.Name);
            Assert.IsTrue(condition: item.CreatedDate == dto.CreatedDate);
            Assert.IsTrue(condition: item.Price == dto.Price);
        }
    }
}
