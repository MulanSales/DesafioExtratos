using System;
using ExtratosApi.Models.Database;
using MongoDB.Driver;
using Moq;

namespace ExtratosApi.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseServicesSchema<TestClass> dbServicesSchema { get; private set; }
        public DatabaseFixture()
        {
            var mockDatabaseSettings = new Mock<IDatabaseConnectorSettings>();
            mockDatabaseSettings.SetupGet(mi => mi.DatabaseName).Returns("extratos-api-test-db");
            mockDatabaseSettings.SetupGet(mi => mi.ConnectionString).Returns("mongodb+srv://rw-permission-user:CgMedcmoNYcJCaYJ@nodecourse-tnt0c.mongodb.net/?ssl=true&authSource=admin&w=majority");
            mockDatabaseSettings.SetupGet(mi => mi.CollectionName).Returns("TestCollection");

            this.dbServicesSchema = new DatabaseServicesSchema<TestClass>(mockDatabaseSettings.Object);
        }
        public void Dispose()
        {
            var filter = FilterDefinition<TestClass>.Empty;
            this.dbServicesSchema._collection.DeleteManyAsync(filter);
        }
    }
    public class TestClass : ICollectionSchema
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}