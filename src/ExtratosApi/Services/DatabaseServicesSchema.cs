using System.Collections.Generic;
using System.Threading.Tasks;
using ExtratosApi.Models.Database;
using MongoDB.Driver;

namespace ExtratosApi.Services{
    public class DatabaseServicesSchema<T> where T : ICollectionSchema
    {
        public IMongoCollection<T> _collection; 

        private IDatabaseConnectorSettings _settings;

        public DatabaseServicesSchema(IDatabaseConnectorSettings settings)
        {
            this._settings = settings;

            var client = new MongoClient(_settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(_settings.DatabaseName);

            string collectionName = typeof(T).Name;

            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T> CreateItem(T collectionItem) {
            await _collection.InsertOneAsync(collectionItem);
            return collectionItem;
        }

        public async Task<List<T>> GetAll() {
            IAsyncCursor<T> itemsList = await _collection.FindAsync(item => true);
            return await itemsList.ToListAsync();
        }

        public async Task<T> GetById(string id) {
            IAsyncCursor<T> collectionItem = await _collection.FindAsync<T>(item => item.Id == id);
            return await collectionItem.FirstOrDefaultAsync();
        }

        public async Task<ReplaceOneResult> UpdateById(string id, T updatedItem) {
            return await _collection.ReplaceOneAsync(item => item.Id == id, updatedItem);
        }

        public async Task<DeleteResult> RemoveById(string id) {
            return await _collection.DeleteOneAsync(item => item.Id == id);
        }

    }
}