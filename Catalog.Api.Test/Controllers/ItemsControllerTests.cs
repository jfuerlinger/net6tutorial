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
using Microsoft.FeatureManagement;
using AutoFixture.Xunit2;
using Catalog.Core;

namespace Catalog.Api.Test.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ItemsControllerTests
    {
        [Theory]
        [AutoDomainData]
        public async Task GetItemsAsync_Call_ShouldReturnFakedDtosAsync(
            [Greedy] ItemsController controller)
        {
            // Arrange
            // Act
            var result = await controller.GetItemsAsync();

            // Test
            Assert.True(result.Any());
        }

        [Theory]
        [AutoDomainData]
        public async Task GetItemAsync_CallWithExistingId_ShouldReturnCorrectDtoAsync(
            [Greedy] ItemsController controller)
        {
            // Arrange
            // Act
            var items = await controller.GetItemsAsync();

            var getItemResult = await controller.GetItemAsync(items.ElementAt(1).Id);
            var getItemOkResult = getItemResult.Result as OkObjectResult;
            var itemDto = getItemOkResult.Value as ItemDto;

            // Assert
            Assert.NotNull(getItemResult);
            Assert.NotNull(getItemOkResult);
            Assert.Equal(StatusCodes.Status200OK, getItemOkResult.StatusCode);
            Assert.IsType<ItemDto>(getItemOkResult.Value);
            Assert.NotNull(itemDto);
            Assert.True(itemDto.Id == items.ElementAt(1).Id);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetItemAsync_CallWithNonExistingId_ShouldReturn404Async(
            [Greedy] ItemsController controller)
        {
            // Arrange
            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<ItemDto>>(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [AutoDomainData]
        public async Task CreateItemAsync_CallWithValidParameters_ShouldReturn201(
            [Greedy] ItemsController controller,
            CreateItemDto createItemDto)
        {
            // Arrange
            // Act
            var createItemResult = await controller.CreateItemAsync(createItemDto);
            var createItemCreatedAtActionResult = createItemResult.Result as CreatedAtActionResult;
            var itemDto = createItemCreatedAtActionResult.Value as ItemDto;

            var getItemResult = await controller.GetItemAsync(itemDto.Id);
            var getItemOkObjectResult = getItemResult.Result as OkObjectResult;
            var itemDtoFromFetch = getItemOkObjectResult.Value as ItemDto;

            // Assert
            Assert.NotNull(createItemResult);
            Assert.NotNull(createItemCreatedAtActionResult);
            Assert.Equal(StatusCodes.Status201Created, createItemCreatedAtActionResult.StatusCode);
            Assert.True(createItemCreatedAtActionResult.RouteValues.Single().Key.Equals("id", StringComparison.InvariantCultureIgnoreCase));
            Assert.NotNull(itemDto);
            Assert.True(Guid.Parse(createItemCreatedAtActionResult.RouteValues.Single().Value.ToString()) == itemDto.Id);
            Assert.IsType<ItemDto>(createItemCreatedAtActionResult.Value);
            Assert.IsType<ItemDto>(getItemOkObjectResult.Value);
            Assert.Equal(createItemDto.Name, itemDtoFromFetch.Name);
            Assert.Equal(createItemDto.Price, itemDtoFromFetch.Price);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteItemAsync_Call_ShouldReturnNoContent(
            [Frozen] Mock<IFeatureManager> featureManagerMock,
            [Greedy] ItemsController controller,
            CreateItemDto createItemDto)
        {
            // Arrange
            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == nameof(Feature.DeleteItemEnabled) ? Task.FromResult(true) : Task.FromResult(false));

            var result = await controller.CreateItemAsync(createItemDto);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var itemDto = createdAtActionResult.Value as ItemDto;

            // Act
            var noContentResult = await controller.DeleteItemAsync(itemDto.Id) as NoContentResult;

            var getItemResult = await controller.GetItemAsync(itemDto.Id);
            var notFoundResult = getItemResult.Result as NotFoundObjectResult;

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
            [Frozen] Mock<IFeatureManager> featureManagerMock,
            [Greedy] ItemsController controller)
        {
            // Arrange
            featureManagerMock
                .Setup(foo => foo.IsEnabledAsync(It.IsAny<string>()))
                .Returns<string>(featureName => featureName == nameof(Feature.DeleteItemEnabled) ? Task.FromResult(true) : Task.FromResult(false));

            // Act
            var notFoundResult = await controller.DeleteItemAsync(Guid.NewGuid()) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateItemAsync_CallWithUnknownId_ShouldReturnNoFound(
            [Greedy] ItemsController controller)
        {
            // Arrange
            // Act
            var notFoundResult = await controller.UpdateItemAsync(Guid.NewGuid(), new UpdateItemDto()) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(notFoundResult);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateItemAsync_CallWithKnownId_ShouldUpdateValue(
            [Greedy] ItemsController controller,
            CreateItemDto newItem,
            UpdateItemDto updateItem)
        {
            // Arrange
            // Act
            var createItemResult = await controller.CreateItemAsync(newItem);
            var createItemResultCreatedAtActionResult = createItemResult.Result as CreatedAtActionResult;
            var createItemResultValue = createItemResultCreatedAtActionResult.Value as ItemDto;

            var newItemId = createItemResultValue.Id;
            var resultUpdate = await controller.UpdateItemAsync(newItemId, updateItem) as NoContentResult;

            var getItemResult = await controller.GetItemAsync(newItemId);
            var getItemResultOkObjectResult = getItemResult.Result as OkObjectResult;
            var getItemResultValue = getItemResultOkObjectResult.Value as ItemDto;

            // Assert
            Assert.NotNull(createItemResult);
            Assert.NotNull(resultUpdate);
            Assert.Equal(StatusCodes.Status204NoContent, resultUpdate.StatusCode);

            Assert.Equal(updateItem.Name, getItemResultValue?.Name);
            Assert.Equal(updateItem.Price, getItemResultValue?.Price);
        }
    }
}
