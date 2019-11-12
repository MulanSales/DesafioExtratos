using ExtratosApi.Models;
using ExtratosApi.Models.Database;

namespace ExtratosApi.Services{
    public class ReleasesService : DatabaseServicesSchema<Release>
    {
        public ReleasesService(IDatabaseConnectorSettings settings) : base(settings)
        {
        }
    }
}