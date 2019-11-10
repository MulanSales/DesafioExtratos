namespace ExtratosApi.Models.Database {
    public class DatabaseConnectorSettings : IDatabaseConnectorSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}