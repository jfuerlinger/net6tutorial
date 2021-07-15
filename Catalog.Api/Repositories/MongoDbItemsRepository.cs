using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository : IItemsRepository
    {
        private const string databaseName = "catalog";
        private const string collectionName = "items";

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        private readonly IMongoCollection<Item> itemsCollection;

        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            itemsCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task CreateItemAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, id);
            await itemsCollection.DeleteOneAsync(filter);

            await Task.CompletedTask;
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return (await itemsCollection.FindAsync(filter))
                        .SingleOrDefault();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return (await itemsCollection.FindAsync(new BsonDocument()))
                        .ToList();
        }

        public async Task UpdateItemAsync(Item item)
        {
            var filter = filterBuilder.Eq(existingItem => existingItem.Id, item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);

            await Task.CompletedTask;
        }
    }

}