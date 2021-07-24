using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Core.Repositories;
using Catalog.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Catalog.Persistence.InMemory
{
    [ExcludeFromCodeCoverage]
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Item> items = new()
        {
            new Item { Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Iron Sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
            new Item { Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 18, CreatedDate = DateTimeOffset.UtcNow }
        };

        public async Task<IEnumerable<Item>> GetItemsAsync()
            => await Task.FromResult(items);

        public async Task<Item> GetItemAsync(Guid id)
            => await Task.FromResult(items
                .SingleOrDefault(item => item.Id == id));

        public async Task CreateItemAsync(Item item)
        {
            items.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(Item item)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == item.Id);
            items[index] = item;
            await Task.CompletedTask;
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var index = items.FindIndex(existingItem => existingItem.Id == id);
            items.RemoveAt(index);

            await Task.CompletedTask;
        }
    }
}