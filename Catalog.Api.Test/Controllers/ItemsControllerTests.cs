using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Catalog.Api.Controllers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Catalog.Core.Repositories;
using Catalog.Core.Entities;
using Microsoft.FeatureManagement;

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
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

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
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

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
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

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
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

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

            var featureManagerMock = new Mock<IFeatureManager>();
            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == "DeleteItemEnabled" ? Task.FromResult(true) : Task.FromResult(false));

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

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

        [TestMethod]
        public async Task DeleteItemAsync_CallWithUnknownId_ShouldReturnNoFound()
        {
            // Arrange
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

            var featureManagerMock = new Mock<IFeatureManager>();
            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == "DeleteItemEnabled" ? Task.FromResult(true) : Task.FromResult(false));

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

            // Act
            var notFoundResult = await controller.DeleteItemAsync(Guid.NewGuid()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateItemAsync_CallWithUnknownId_ShouldReturnNoFound()
        {
            // Arrange
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
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

            // Act
            var notFoundResult = await controller.UpdateItemAsync(Guid.NewGuid(), new UpdateItemDto()) as NotFoundResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateItemAsync_CallWithKnownId_ShouldUpdateValue()
        {
            // Arrange
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
                .Setup(foo => foo.GetItemsAsync())
                .Returns(() =>
                {
                    return Task.FromResult(items.Values.AsEnumerable());
                });

            itemsRepositoryMock
                .Setup(foo => foo.UpdateItemAsync(It.IsNotNull<Item>()))
                .Returns<Item>((item) =>
                {
                    items[item.Id] = item;
                    return Task.CompletedTask;
                });

            itemsRepositoryMock
                .Setup(foo => foo.DeleteItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>((id) =>
                {
                    items.Remove(id);
                    return Task.CompletedTask;
                });


            var loggerMock = new Mock<ILogger<ItemsController>>();
            var featureManagerMock = new Mock<IFeatureManager>();

            var controller = new ItemsController(
                loggerMock.Object,
                featureManagerMock.Object,
                itemsRepositoryMock.Object);

            var newItem = new CreateItemDto()
            {
                Name = "item 1",
                Price = 24
            };



            // Act
            var createItemResult = await controller.CreateItemAsync(newItem);
            var createItemResultCreatedAtActionResult = createItemResult.Result as CreatedAtActionResult;
            var createItemResultValue = createItemResultCreatedAtActionResult.Value as ItemDto;

            var newItemId = createItemResultValue.Id;
            var resultUpdate = await controller.UpdateItemAsync(newItemId, new UpdateItemDto() { Name = "item 2", Price = 20 }) as NoContentResult;
            
            var getItemResult = await controller.GetItemAsync(newItemId);
            var getItemResultOkObjectResult = getItemResult.Result as OkObjectResult;
            var getItemResultValue = getItemResultOkObjectResult.Value as ItemDto;

            Assert.IsNotNull(createItemResult);
            Assert.IsNotNull(resultUpdate);
            Assert.AreEqual(StatusCodes.Status204NoContent, resultUpdate.StatusCode);

            Assert.AreEqual("item 2", getItemResultValue?.Name);
            Assert.AreEqual(20, getItemResultValue?.Price);
        }
    }
}
