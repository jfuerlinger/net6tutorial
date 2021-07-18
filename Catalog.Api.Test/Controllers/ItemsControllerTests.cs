using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;


namespace Catalog.Api.Test.Controllers
{
    [TestClass]
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
    }
}
