using System;
using System.Threading.Tasks;
using ExtratosApi.Controllers;
using ExtratosApi.Models.Database;
using ExtratosApi.Tests.Fixtures;
using Moq;
using Xunit;

namespace ExtratosApi.Tests
{
    public class DatabaseConnectionTests : IClassFixture<DatabaseFixture>
    {
        public DatabaseFixture fixture;
        public DatabaseConnectionTests(DatabaseFixture fixture) {
            this.fixture = fixture;
        }

        [Fact]
        public void Test1()
        {
            var controller = new ValuesController();
            var result = controller.Get();

            Assert.Equal("value1", result.Value[0]);
        }

        [Fact]
        public async Task ConnectToDatabase_WithSuccess_Test() {
            DateTime testItemCreationTime = DateTime.Now;
            var testItem = await fixture.dbServicesSchema.CreateItem(new TestClass {CreatedAt = testItemCreationTime, UpdatedAt = DateTime.Now});
            Assert.Equal(testItemCreationTime, testItem.CreatedAt);
        }

    }
}
