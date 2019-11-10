using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ExtratosApi.Models.Database {
    public class DatabaseServicesSchema<T> where T : ICollectionSchema
    {
        protected IMongoCollection<T> _collection; 

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

        public async Task<List<T>> GetAll(string id) {
            IAsyncCursor<T> itemsList = await _collection.FindAsync(item => true);
            return await itemsList.ToListAsync();
        }

        public async Task<T> GetById(string id) {
            IAsyncCursor<T> collectionItem = await _collection.FindAsync<T>(item => item.Id == id);
            return await collectionItem.FirstOrDefaultAsync();
        }

        public async void UpdateById(string id, T updatedItem) {
            await _collection.ReplaceOneAsync(item => item.Id == id, updatedItem);
        }

        public async void RemoveById(string id) {
            await _collection.DeleteOneAsync(item => item.Id == id);
        }

    }
}