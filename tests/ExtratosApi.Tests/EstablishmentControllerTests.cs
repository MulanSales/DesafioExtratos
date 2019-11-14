using System;
using System.Collections.Generic;
using ExtratosApi.Controllers;
using ExtratosApi.Models;
using ExtratosApi.Models.Database;
using ExtratosApi.Models.Request;
using ExtratosApi.Services;
using ExtratosApi.Tests.Fixtures;
using ExtratosApi.Tests.Wrappers;
using Moq;
using Xunit;

namespace ExtratosApi.Tests
{
    [Collection("Establishment Controller Tests")]
    public class EstablishmentControllerTests : IClassFixture<DatabaseSettingsFixture>
    {
        private readonly IDatabaseConnectorSettings dbSettings;
        private readonly LoggerWrapper<EstablishmentsController> loggerWrapper;
        private readonly EstablishmentService establishmentService;
        private readonly ControllerMessages controllerMessages;
        private readonly EstablishmentsController establishmentsController;

        public EstablishmentControllerTests(DatabaseSettingsFixture dbFixture)
        {   
            // 0: Setting wrapper for logger
            loggerWrapper = new LoggerWrapper<EstablishmentsController>();

            // 1: Setting establishment service given db settings
            this.dbSettings = dbFixture.dbSettings;
            this.establishmentService = dbFixture.establishmentService; 

             // 2: Get controller messages
            this.controllerMessages = GetControllerMessagesProperties();

            // 3: Instantiate of Establishment Controller
            this.establishmentsController = new EstablishmentsController(loggerWrapper, establishmentService, controllerMessages);
        }

        [Fact(DisplayName = "Should returns 404 if there is no establishment on database")]
        public async void Get_StatusCode_404_Test() 
        {
            // 0: Remove all establishments from database
            await establishmentService.RemoveAll();

            // 1: Call GET Action
            var query = await establishmentsController.Get();

            var result = query.Result.GetType().GetProperty("Value").GetValue(query.Result);
            Type resultType = result.GetType();

            Assert.Equal(404, (int)resultType.GetProperty("StatusCode").GetValue(result));
            Assert.Equal(controllerMessages.NotFound.Replace("$", "Estabelecimento"), (string)resultType.GetProperty("Message").GetValue(result));
        }

        [Fact(DisplayName = "Should throws if any exception occurs while GET")]
        public async void Get_ThrowsException_Test()
        {
            // 1: Mocking GetAll Method to throws
            var establishmentServiceMock = new Mock<EstablishmentService>(dbSettings);
            establishmentServiceMock.Setup(es => es.GetAll()).ThrowsAsync(new InvalidOperationException());

            var establishmentsControllerLocal = new EstablishmentsController(loggerWrapper, establishmentServiceMock.Object, controllerMessages);

            // 2: Call GET Action and Expects to throws
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await establishmentsControllerLocal.Get());
        }

        [Fact(DisplayName = "Should return 200 and a list of establishments if successful")]
        public async void Get_SuccessStatus200_Test()
        {
            // 1: Creating testing objects
            Establishment firstEstablishment = new Establishment() 
            {
                Name = "Test 1",
                Type = "Alimentação"
            };

            Establishment secondEstablishment = new Establishment()
            {
                Name = "Test 2",
                Type = "Alimentação"
            };

            // 2: Adding to database
            await establishmentService.CreateItem(firstEstablishment);
            await establishmentService.CreateItem(secondEstablishment);

            var query = await establishmentsController.Get();

            var result = (List<Establishment>)query.Result.GetType().GetProperty("Value").GetValue(query.Result);

            // 3: Check if result is a valid Establishment
            Assert.Equal("Test 1", result[0].Name);
            Assert.Equal("Test 2", result[1].Name);
        }

        [Fact(DisplayName = "Should returns 406 if an establishment with given name already exists")]
        public async void Post_NotAccepted406_Test()
        {
            // 1: Creating testing objects
            Establishment testEstablishment = new Establishment() 
            {
                Name = "Test 1",
                Type = "Alimentação"
            };

            // 2: Adding to database synchronously
            await establishmentService.CreateItem(testEstablishment);

            // 3: Call POST Action passing body request with an establishment which already exists
            var query = await establishmentsController.Post(new EstablishmentRequest {
                Name = "Test 1",
                Type = "Alimentação"
            });

            var result = query.Result.GetType().GetProperty("Value").GetValue(query.Result);
            Type resultType = result.GetType();

            Assert.Equal(406, (int)resultType.GetProperty("StatusCode").GetValue(result));
            Assert.Equal(controllerMessages.NotAccepted.Replace("$", "estabelecimento"), (string)resultType.GetProperty("Message").GetValue(result));
        }

        [Fact(DisplayName = "Should throws if any exception occurs while POST")]
        public async void Post_ThrowsException_Test()
        {
            // 0: Remove all establishments from database
            await establishmentService.RemoveAll();

            // 1: Request body
            var requestBody = new EstablishmentRequest {
                Name = "Test 1",
                Type = "Alimentação"
            };

            // 2: Mocking GetByName Method to throws
            var establishmentServiceMock = new Mock<EstablishmentService>(dbSettings);
            establishmentServiceMock.Setup(es => es.GetByName(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());

            var establishmentsControllerLocal = new EstablishmentsController(loggerWrapper, establishmentServiceMock.Object, controllerMessages);

            // 3: Call POST Action and Expects to throws
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await establishmentsControllerLocal.Post(requestBody));
        }

        [Fact(DisplayName = "Should return 201 and the establishment created")]
        public async void Post_SuccessStatus201_Test()
        {
            // 0: Remove all establishments from database
            await establishmentService.RemoveAll();

            // 1: Request body
            var requestBody = new EstablishmentRequest {
                Name = "Test 1",
                Type = "alimentação"
            };

            // 2: Call POST Action passing body request with a new establishment
            var query = await establishmentsController.Post(requestBody);

            var resultStatusCode = query.Result.GetType().GetProperty("StatusCode").GetValue(query.Result);
            var resultValue = query.Result.GetType().GetProperty("Value").GetValue(query.Result);

            Assert.Equal(201, (int)resultStatusCode);
            Assert.Equal("Test 1", ((Establishment)resultValue).Name);
            Assert.Equal("Alimentação", ((Establishment)resultValue).Type);
        }


        private ControllerMessages GetControllerMessagesProperties() 
        {
            return new ControllerMessages() 
            {
                NotFound = "Não foi possível encontrar nenhum $ no banco de dados.",
                NotAccepted = "Não é permitido inserir $, pois já existe um $ cadastrado com esse nome.",
                IncorretIdFormat = "O paramêtro Id está em formato incorreto. Deve ser hexadecimal com tamanho 24",
                NotFoundGivenId = "Não foi possível encontrar nenhum $ associado com esse id.",
                CantUpdate = "Não foi possível realizar a atualização seguindo os valores passados.",
                CantFoundGivenName = "Não foi possível encontrar nenhum $ associado com esse nome.",
                CantRemove = "Não foi possível realizar a remoção seguindo os valores passados.",
                DeletedSuccess= "$ deletado com sucesso"
            };
        }
        
    }
}