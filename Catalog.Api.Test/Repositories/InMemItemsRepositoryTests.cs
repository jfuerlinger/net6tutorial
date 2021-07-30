using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Persistence.InMemory;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.Api.Test.Repositories
{
    public class InMemItemsRepositoryTests
    {
        [Theory]
        [AutoDomainData]
        public async Task GetItemsAsync_Call_ShouldReturnCorrectItems(
            InMemItemsRepository repo)
        {
            // Arrange
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

        [Theory]
        [AutoDomainData]
        public async Task CreateItemAsync_Call_ShouldStoreCorrectItem(
            Item newItem,
            InMemItemsRepository repo)
        {
            // Arrange
            // Act
            await repo.CreateItemAsync(newItem);
            var createdItem = await repo.GetItemAsync(newItem.Id);

            // Assert
            Assert.NotNull(createdItem);
            Assert.Same(newItem, createdItem);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteItemAsync_Call_ShouldRemoveItem(
            Item newItem,
            InMemItemsRepository repo)
        {
            // Arrange
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
