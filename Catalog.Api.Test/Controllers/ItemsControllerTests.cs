using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Catalog.Api.Test.Controllers
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ItemsControllerTests
    {
        [TestMethod]
        public async Task GetItemsAsync_Call_ShouldReturnFakedDtosAsync()
        {
            // Arrange

            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            var itemsRepositoryMock = new Mock<IItemsRepository>();
            itemsRepositoryMock.Setup(foo => foo.GetItemsAsync().Result)
                .Returns(items);

            var loggerMock = new Mock<ILogger<ItemsController>>();

            var controller = new ItemsController(
                itemsRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var result = await controller.GetItemsAsync();

            // Test
            Assert.IsTrue(result.Count() == 3);
        }

        [TestMethod]
        public async Task GetItemAsync_CallWithExistingId_ShouldReturnCorrectDtoAsync()
        {
            // Arrange
            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            var itemsRepositoryMock = new Mock<IItemsRepository>();
            itemsRepositoryMock
                .Setup(foo => foo.GetItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>((id) => Task.FromResult(items.FirstOrDefault(item => item.Id == id)));

            var loggerMock = new Mock<ILogger<ItemsController>>();

            var controller = new ItemsController(
                itemsRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var result = await controller.GetItemAsync(items.ElementAt(1).Id);
            var okResult = result.Result as OkObjectResult;
            var itemDto = okResult.Value as ItemDto;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.GetItemAsync(It.IsAny<Guid>()), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemDto));
            Assert.IsNotNull(itemDto);
            Assert.IsTrue(itemDto.Id == items.ElementAt(1).Id);
        }

        [TestMethod]
        public async Task GetItemAsync_CallWithNonExistingId_ShouldReturn404Async()
        {
            // Arrange
            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            var itemsRepositoryMock = new Mock<IItemsRepository>();
            itemsRepositoryMock
                .Setup(foo => foo.GetItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>(id => Task.FromResult(items.FirstOrDefault(item => item.Id == id)));

            var loggerMock = new Mock<ILogger<ItemsController>>();

            var controller = new ItemsController(
                itemsRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            var notFoundResult = result.Result as NotFoundResult;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.GetItemAsync(It.IsAny<Guid>()), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateItemAsync_CallWithValidParameters_ShouldReturn201()
        {
            // Arrange
            CreateItemDto createItemDto = new CreateItemDto()
            {
                Name = "item 1",
                Price = 15
            };

            Dictionary<Guid, Item> items = new();

            var itemsRepositoryMock = new Mock<IItemsRepository>();
            itemsRepositoryMock
                .Setup(foo => foo.CreateItemAsync(It.IsNotNull<Item>()))
                .Returns<Item>((item) =>
                {
                    items[item.Id] = item;
                    return Task.CompletedTask;
                });

            var loggerMock = new Mock<ILogger<ItemsController>>();

            var controller = new ItemsController(
                itemsRepositoryMock.Object,
                loggerMock.Object);

            // Act
            var result = await controller.CreateItemAsync(createItemDto);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var itemDto = createdAtActionResult.Value as ItemDto;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.CreateItemAsync(It.IsAny<Item>()), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.IsTrue(createdAtActionResult.RouteValues.Single().Key.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Guid.Parse(createdAtActionResult.RouteValues.Single().Value.ToString()) == items.Single().Value.Id);
            Assert.IsInstanceOfType(createdAtActionResult.Value, typeof(ItemDto));
            Assert.IsNotNull(itemDto);
            Assert.IsTrue(items.Count == 1);
            Assert.IsTrue(items.First().Value.Id == itemDto.Id);
        }

        [TestMethod]
        public async Task DeleteItemAsync_Call_ShouldReturnNoContent()
        {
            // Arrange
            CreateItemDto createItemDto = new CreateItemDto()
            {
                Name = "item 1",
                Price = 15
            };

            Dictionary<Guid, Item> items = new();

            var itemsRepositoryMock = new Mock<IItemsRepository>();
            itemsRepositoryMock
                .Setup(foo => foo.CreateItemAsync(It.IsNotNull<Item>()))
                .Returns<Item>((item) =>
                {
                    items[item.Id] = item;
                    return Task.CompletedTask;
                });

            itemsRepositoryMock
                .Setup(foo => foo.GetItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>((id) =>
                {
                    if (items.ContainsKey(id))
                    {
                        return Task.FromResult(items[id]);
                    }
                    else
                    {
                        return Task.FromResult(default(Item));
                    }
                });

            itemsRepositoryMock
                .Setup(foo => foo.DeleteItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>((id) =>
                {
                    items.Remove(id);
                    return Task.CompletedTask;
                });


            var loggerMock = new Mock<ILogger<ItemsController>>();

            var controller = new ItemsController(
                itemsRepositoryMock.Object,
                loggerMock.Object);

            var result = await controller.CreateItemAsync(createItemDto);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var itemDto = createdAtActionResult.Value as ItemDto;

            // Act
            var noContentResult = await controller.DeleteItemAsync(itemDto.Id) as NoContentResult;

            var getItemResult = await controller.GetItemAsync(itemDto.Id);
            var notFoundResult = getItemResult.Result as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.IsTrue(createdAtActionResult.RouteValues.Single().Key.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Guid.Parse(createdAtActionResult.RouteValues.Single().Value.ToString()) == itemDto.Id);
            Assert.IsInstanceOfType(createdAtActionResult.Value, typeof(ItemDto));
            Assert.IsNotNull(itemDto);
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(noContentResult.StatusCode, StatusCodes.Status204NoContent);
            Assert.AreEqual(notFoundResult.StatusCode, StatusCodes.Status404NotFound);
        }
    }
}
