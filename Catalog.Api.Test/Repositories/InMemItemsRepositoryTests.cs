using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Api.Test.Repositories
{
    [TestClass]
    public class InMemItemsRepositoryTests
    {
        [TestMethod]
        public async Task GetItemsAsync_Call_ShouldReturnCorrectItems()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            // Act
            var items = await repo.GetItemsAsync();

            // Assert
            Assert.IsNotNull(items);
            Assert.AreEqual(3, items.Count());
        }

        [TestMethod]
        public async Task GetItemAsync_Call_ShouldReturnCorrectItem()
        {
            // Arrange
            IItemsRepository repo = new InMemItemsRepository();

            // Act
            var items = await repo.GetItemsAsync();
            var firstItem = await repo.GetItemAsync(items.First().Id);

            // Assert
            Assert.IsNotNull(items);
            Assert.IsNotNull(firstItem);
            Assert.AreEqual(firstItem.Id, items.First().Id);
        }

        [TestMethod]
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
            Assert.IsNotNull(createdItem);
            Assert.AreSame(newItem, createdItem);
        }

        [TestMethod]
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
            Assert.IsNotNull(createdItem);
            Assert.AreSame(newItem, createdItem);
            Assert.IsNull(createdItemAfterRemoval);
        }

    }
}
