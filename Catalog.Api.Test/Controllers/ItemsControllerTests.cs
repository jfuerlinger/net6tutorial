using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
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
using AutoFixture.Xunit2;

namespace Catalog.Api.Test.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ItemsControllerTests
    {
        [Theory]
        [AutoDomainData]
        public async Task GetItemsAsync_Call_ShouldReturnFakedDtosAsync(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            itemsRepositoryMock.Setup(foo => foo.GetItemsAsync().Result)
                .Returns(items);

            // Act
            var result = await controller.GetItemsAsync();

            // Test
            Assert.True(result.Count() == 3);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetItemAsync_CallWithExistingId_ShouldReturnCorrectDtoAsync(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            itemsRepositoryMock
                .Setup(foo => foo.GetItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>((id) => Task.FromResult(items.FirstOrDefault(item => item.Id == id)));

            // Act
            var result = await controller.GetItemAsync(items.ElementAt(1).Id);
            var okResult = result.Result as OkObjectResult;
            var itemDto = okResult.Value as ItemDto;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.GetItemAsync(It.IsAny<Guid>()), Times.Once);

            Assert.NotNull(result);
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsType<ItemDto>(okResult.Value);
            Assert.NotNull(itemDto);
            Assert.True(itemDto.Id == items.ElementAt(1).Id);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetItemAsync_CallWithNonExistingId_ShouldReturn404Async(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            var items = (new Item[] {
                new Item() { Id = Guid.NewGuid(),  Name="item 1"},
                new Item() { Id = Guid.NewGuid(),  Name="item 2"},
                new Item() { Id = Guid.NewGuid(),  Name="item 3"}
            }).AsEnumerable();

            itemsRepositoryMock
                .Setup(foo => foo.GetItemAsync(It.IsNotNull<Guid>()))
                .Returns<Guid>(id => Task.FromResult(items.FirstOrDefault(item => item.Id == id)));

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            var notFoundResult = result.Result as NotFoundResult;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.GetItemAsync(It.IsAny<Guid>()), Times.Once);

            Assert.NotNull(result);
            Assert.NotNull(notFoundResult);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Theory]
        [AutoDomainData]
        public async Task CreateItemAsync_CallWithValidParameters_ShouldReturn201(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller,
            CreateItemDto createItemDto)
        {
            // Arrange
            Dictionary<Guid, Item> items = new();

            itemsRepositoryMock
                .Setup(foo => foo.CreateItemAsync(It.IsNotNull<Item>()))
                .Returns<Item>((item) =>
                {
                    items[item.Id] = item;
                    return Task.CompletedTask;
                });

            // Act
            var result = await controller.CreateItemAsync(createItemDto);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var itemDto = createdAtActionResult.Value as ItemDto;

            // Assert
            itemsRepositoryMock.Verify(foo => foo.CreateItemAsync(It.IsAny<Item>()), Times.Once);

            Assert.NotNull(result);
            Assert.NotNull(createdAtActionResult);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.True(createdAtActionResult.RouteValues.Single().Key.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Guid.Parse(createdAtActionResult.RouteValues.Single().Value.ToString()) == items.Single().Value.Id);
            Assert.IsType<ItemDto>(createdAtActionResult.Value);
            Assert.NotNull(itemDto);
            Assert.True(items.Count == 1);
            Assert.True(items.First().Value.Id == itemDto.Id);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteItemAsync_Call_ShouldReturnNoContent(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Frozen] Mock<IFeatureManager> featureManagerMock,
            [Greedy] ItemsController controller,
            CreateItemDto createItemDto)
        {
            // Arrange
            Dictionary<Guid, Item> items = new();

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

            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == "DeleteItemEnabled" ? Task.FromResult(true) : Task.FromResult(false));

            var result = await controller.CreateItemAsync(createItemDto);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var itemDto = createdAtActionResult.Value as ItemDto;

            // Act
            var noContentResult = await controller.DeleteItemAsync(itemDto.Id) as NoContentResult;

            var getItemResult = await controller.GetItemAsync(itemDto.Id);
            var notFoundResult = getItemResult.Result as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(createdAtActionResult);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            Assert.True(createdAtActionResult.RouteValues.Single().Key.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            Assert.True(Guid.Parse(createdAtActionResult.RouteValues.Single().Value.ToString()) == itemDto.Id);
            Assert.IsType<ItemDto>(createdAtActionResult.Value);
            Assert.NotNull(itemDto);
            Assert.NotNull(noContentResult);
            Assert.Equal(noContentResult.StatusCode, StatusCodes.Status204NoContent);
            Assert.Equal(notFoundResult.StatusCode, StatusCodes.Status404NotFound);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteItemAsync_CallWithFeatureDisabled_ShouldReturnBadRequest(
            [Frozen] Mock<IFeatureManager> featureManagerMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => Task.FromResult(false));

            // Act
            var badRequestObjectResult = await controller.DeleteItemAsync(Guid.NewGuid()) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
            Assert.Equal("This feature ist not released yet!", badRequestObjectResult.Value);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteItemAsync_CallWithUnknownId_ShouldReturnNoFound(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Frozen] Mock<IFeatureManager> featureManagerMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            Dictionary<Guid, Item> items = new();

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


            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == "DeleteItemEnabled" ? Task.FromResult(true) : Task.FromResult(false));

            // Act
            var notFoundResult = await controller.DeleteItemAsync(Guid.NewGuid()) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateItemAsync_CallWithUnknownId_ShouldReturnNoFound(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            Dictionary<Guid, Item> items = new();

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

            // Act
            var notFoundResult = await controller.UpdateItemAsync(Guid.NewGuid(), new UpdateItemDto()) as NotFoundResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateItemAsync_CallWithKnownId_ShouldUpdateValue(
            [Frozen] Mock<IItemsRepository> itemsRepositoryMock,
            [Greedy] ItemsController controller,
            CreateItemDto newItem,
            UpdateItemDto updateItem)
        {
            // Arrange
            Dictionary<Guid, Item> items = new();

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
            

            // Act
            var createItemResult = await controller.CreateItemAsync(newItem);
            var createItemResultCreatedAtActionResult = createItemResult.Result as CreatedAtActionResult;
            var createItemResultValue = createItemResultCreatedAtActionResult.Value as ItemDto;

            var newItemId = createItemResultValue.Id;
            var resultUpdate = await controller.UpdateItemAsync(newItemId, updateItem) as NoContentResult;

            var getItemResult = await controller.GetItemAsync(newItemId);
            var getItemResultOkObjectResult = getItemResult.Result as OkObjectResult;
            var getItemResultValue = getItemResultOkObjectResult.Value as ItemDto;

            Assert.NotNull(createItemResult);
            Assert.NotNull(resultUpdate);
            Assert.Equal(StatusCodes.Status204NoContent, resultUpdate.StatusCode);

            Assert.Equal(updateItem.Name, getItemResultValue?.Name);
            Assert.Equal(updateItem.Price, getItemResultValue?.Price);
        }
    }
}
