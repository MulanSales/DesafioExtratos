using System.Threading.Tasks;
using ExtratosApi.Models;
using ExtratosApi.Models.Database;
using MongoDB.Driver;

namespace ExtratosApi.Services{
    public class EstablishmentService: DatabaseServicesSchema<Establishment>
    {
        public EstablishmentService(IDatabaseConnectorSettings settings) : base(settings)
        {
        }

        public async Task<Establishment> GetByName(string name) {
            IAsyncCursor<Establishment> collectionItem = await _collection.FindAsync<Establishment>(item => item.Name == name);
            return await collectionItem.FirstOrDefaultAsync();
        }
    }
}