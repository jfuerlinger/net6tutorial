using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Catalog.Api.Dtos;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Catalog.Core.Repositories;
using Catalog.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IFeatureManager _featureManager;

        private readonly IItemsRepository _repository;

        public ItemsController(
            ILogger<ItemsController> logger,
            IFeatureManager featureManager,
            IItemsRepository repository)
        {
            this._repository = repository;
            this._logger = logger;
            _featureManager = featureManager;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await _repository
                .GetItemsAsync())
                .Select(item => item.AsDto());

            _logger.LogInformation($"{DateTime.UtcNow:hh:mm:ss}: Fetched {items.Count()} items");

            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await _repository
                        .GetItemAsync(id);

            if (item is null)
            {
                _logger.LogWarning($"{DateTime.UtcNow:hh:mm:ss}: The id {id} does not exist!");

                return NotFound(id);
            }

            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {

            var existingItem = await _repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound(id);
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            await _repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }


        [HttpDelete("{id}")]
        [FeatureGate("DeleteItemEnabled")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            bool isDeleteItemEnabled = await _featureManager.IsEnabledAsync("DeleteItemEnabled");
            if(!isDeleteItemEnabled)
            {
                return BadRequest("This feature ist not released yet!");
            }


            var existingItem = await _repository.GetItemAsync(id);
            if (existingItem is null)
            {
                return NotFound(id);
            }

            await _repository.DeleteItemAsync(existingItem.Id);

            return NoContent();
        }

    }
}