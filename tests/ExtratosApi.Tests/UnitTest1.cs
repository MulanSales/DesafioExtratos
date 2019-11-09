using System;
using ExtratosApi.Controllers;
using Xunit;

namespace ExtratosApi.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var controller = new ValuesController();
            var result = controller.Get();

            Assert.Equal("value1", result.Value[0]);
        }
    }
}
