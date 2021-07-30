using Catalog.Core.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Catalog.Api.Test
{
    [ExcludeFromCodeCoverage]
    public class ExtensionsTests
    {
        [Fact]
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
            Assert.True(condition: item.Id == dto.Id);
            Assert.True(condition: item.Name == dto.Name);
            Assert.True(condition: item.CreatedDate == dto.CreatedDate);
            Assert.True(condition: item.Price == dto.Price);
        }
    }
}
