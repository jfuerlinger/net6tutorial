using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Persistence.MongoDb
{
    [ExcludeFromCodeCoverage]
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string _databaseName = "catalog";
        private const string _collectionName = "items";

        private readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

        private readonly IMongoCollection<Item> _itemsCollection;

        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(_databaseName);
            _itemsCollection = database.GetCollection<Item>(_collectionName);
        }

        public async Task CreateItemAsync(Item item)
        {
            await _itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, id);
            await _itemsCollection.DeleteOneAsync(filter);

            await Task.CompletedTask;
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = _filterBuilder.Eq(item => item.Id, id);
            return (await _itemsCollection.FindAsync(filter))
                        .SingleOrDefault();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return (await _itemsCollection.FindAsync(new BsonDocument()))
                        .ToList();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = _filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await _itemsCollection.ReplaceOneAsync(filter, item);

            await Task.CompletedTask;
        }
    }

}