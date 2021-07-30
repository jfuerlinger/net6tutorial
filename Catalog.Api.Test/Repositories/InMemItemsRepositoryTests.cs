using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Persistence.InMemory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.Api.Test.Repositories
{
    public class InMemItemsRepositoryTests
    {
        [Fact]
        public async Task GetItemsAsync_Call_ShouldReturnCorrectItems()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            // Act
            var items = await repo.GetItemsAsync();

            // Assert
            Assert.NotNull(items);
            Assert.Equal(3, items.Count());
        }

        [Fact]
        public async Task GetItemAsync_Call_ShouldReturnCorrectItem()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            // Act
            var items = await repo.GetItemsAsync();
            var firstItem = await repo.GetItemAsync(items.First().Id);

            // Assert
            Assert.NotNull(items);
            Assert.NotNull(firstItem);
            Assert.Equal(firstItem.Id, items.First().Id);
        }

        [Fact]
        public async Task CreateItemAsync_Call_ShouldStoreCorrectItem()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            Item newItem = new()
            {
                Id = Guid.NewGuid(),
                Name = "new item",
                CreatedDate = DateTime.Now,
                Price = 18
            };

            // Act
            await repo.CreateItemAsync(newItem);
            var createdItem = await repo.GetItemAsync(newItem.Id);

            // Assert
            Assert.NotNull(createdItem);
            Assert.Same(newItem, createdItem);
        }

        [Fact]
        public async Task DeleteItemAsync_Call_ShouldRemoveItem()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            Item newItem = new()
            {
                Id = Guid.NewGuid(),
                Name = "new item",
                CreatedDate = DateTime.Now,
                Price = 18
            };

            // Act
            await repo.CreateItemAsync(newItem);
            var createdItem = await repo.GetItemAsync(newItem.Id);
            await repo.DeleteItemAsync(newItem.Id);
            var createdItemAfterRemoval = await repo.GetItemAsync(newItem.Id);

            // Assert
            Assert.NotNull(createdItem);
            Assert.Same(newItem, createdItem);
            Assert.Null(createdItemAfterRemoval);
        }

    }
}
