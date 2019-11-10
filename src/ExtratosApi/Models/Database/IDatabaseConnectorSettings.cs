namespace ExtratosApi.Models.Database {
    public interface IDatabaseConnectorSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        
    }
}