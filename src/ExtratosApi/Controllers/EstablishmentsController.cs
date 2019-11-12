using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExtratosApi.Models;
using ExtratosApi.Models.Request;
using ExtratosApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ExtratosApi.Controllers
{
    [Produces("application/json")]
    [Route("api/establishments")]
    [ApiController]
    public class EstablishmentsController: ControllerBase
    {
        private readonly ILogger<EstablishmentsController> logger;
        private readonly EstablishmentService establishmentService;
        public EstablishmentsController(ILogger<EstablishmentsController> logger, EstablishmentService establishmentService) {
            this.logger = logger;
            this.establishmentService = establishmentService;
        }

        /// <summary>
        /// Returns an array of establishments
        /// </summary>
        /// <returns>An array of all establishments inserted in the past</returns>
        /// <response code="200">Returns the array</response>
        /// <response code="404">If can't find any establishment</response>    
        /// <response code="500">If an error in server side happens</response> 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Establishment>>> Get()
        {
            List<Establishment> establishments;
            try {
                logger.LogInformation("Trying to get establishments from database");
                establishments = await establishmentService.GetAll();

                if (establishments.Count == 0) {
                    var errorDetails = new ResponseDetails() {
                       Message = "Não foi possível encontrar nenhum Estabelecimento no banco de dados.",
                       StatusCode = 404
                   };
                   logger.LogInformation("Error: " + errorDetails.Message);
                  return NotFound(errorDetails);
               }
            } 
            catch (Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            logger.LogInformation("Action GET for /api/establishments returns 200");
            return Ok(establishments);
        }
        
        /// <summary>
        /// Creates a new establishment 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/establishments
        ///     {
        ///        "name": "Padaria Stn",
        ///        "type": "Alimentação",
        ///     }
        ///
        /// </remarks>
        /// <returns>The newly release created</returns>
        /// <response code="201">Returns the newly created establishment</response>
        /// <response code="400">If the request is not in correct format</response>    
        /// <response code="500">If an error in server side happens</response>    
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Establishment>> Post([FromBody] EstablishmentRequest body)
        {
            Establishment resultEstablishment;
            try {
                logger.LogInformation("Trying to verify if establishment with given Name exists");
                var establishment = await establishmentService.GetByName(body.Name.FirstCharToUpper());

                if (establishment != null) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não é permitido inserir estabelecimento, pois já existe um estabelecimento cadastrado com esse nome.",
                        StatusCode = 406
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return StatusCode(406, errorDetails);
                }

                logger.LogInformation("Inserting establishment into database");
                var newEstablishment = new Establishment() {
                    Name = body.Name.FirstCharToUpper(),                       
                    Type = body.Type,
                    CreatedAt = DateTime.Now
                };
                
                resultEstablishment = await establishmentService.CreateItem(newEstablishment);

            } catch(Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            logger.LogInformation("Action POST for /api/establishments returns 201");
            return Created("", resultEstablishment);
        }

        /// <summary>
        /// Updates an old establishment 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/establishments/5dcaad2526235a471cfcccaf
        ///     {
        ///        "name": "Padaria Nova Stn",
        ///        "type": "Alimentação",
        ///     }
        ///
        /// </remarks>
        /// <returns>The updated establishment</returns>
        /// <response code="200">Returns the updated establishment</response>
        /// <response code="400">If the request is not in correct format</response>    
        /// <response code="404">If the resource is not in the database</response>    
        /// <response code="406">If the resource is not acceptable</response>    
        /// <response code="500">If an error in server side happens</response>    
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Establishment>> Put(string id, [FromBody] EstablishmentRequest body)
        {
            // Validating id
            if (!Regex.IsMatch(id, "^[0-9a-fA-F]{24}$")) {
                var errorDetails = new ResponseDetails() {
                        Message = "O paramêtro Id está em formato incorreto. Deve ser hexadecimal com tamanho 24",
                        StatusCode = 400
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return BadRequest(errorDetails);
            }

            Establishment updatedEstablishment;
            try {
                logger.LogInformation("Trying to get a establishemnt with given id");
                var actualEstablishment = await establishmentService.GetById(id);

                if (actualEstablishment == null) {
                     var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum lançamento associado com esse id.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                updatedEstablishment = new Establishment() {
                    Id = id,
                    Name = body.Name.FirstCharToUpper(),
                    Type = body.Type,
                    CreatedAt = actualEstablishment.CreatedAt,
                    UpdatedAt = DateTime.Now
                };

                logger.LogInformation("Trying to update establishment with id: " + id);
                var replaceResult = await establishmentService.UpdateById(id, updatedEstablishment);

                if (!replaceResult.IsAcknowledged) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível realizar a atualização seguindo os valores passados.",
                        StatusCode = 406
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return StatusCode(406, errorDetails);
                }

            } catch (Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            logger.LogInformation("Action PUT for /api/establishments returns 200");
            return Ok(updatedEstablishment);
        }

        /// <summary>
        /// Deletes an establishment 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/establishments/5dcaad2526235a471cfcccaf
        ///
        /// </remarks>
        /// <returns>The response message with success</returns>
        /// <response code="200">Returns sucess response message</response>
        /// <response code="400">If the request is not in correct format</response>    
        /// <response code="404">If the resource is not in the database</response>    
        /// <response code="406">If the resource is not acceptable</response>    
        /// <response code="500">If an error in server side happens</response>    
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDetails>> Delete(string id)
        {
            if (!Regex.IsMatch(id, "^[0-9a-fA-F]{24}$")) {
                var errorDetails = new ResponseDetails() {
                        Message = "O paramêtro Id está em formato incorreto. Deve ser hexadecimal com tamanho 24",
                        StatusCode = 400
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return BadRequest(errorDetails);
            }

            try {
                logger.LogInformation("Trying to get a establishment with given id");
                var actualEstablishment = await establishmentService.GetById(id);

                if (actualEstablishment == null) {
                     var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum estabelecimento associado com esse id.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                var deleteResult = await establishmentService.RemoveById(id);
                if(!deleteResult.IsAcknowledged) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível realizar a remoção seguindo os valores passados.",
                        StatusCode = 406
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return StatusCode(406, errorDetails);
                }

            } catch(Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            logger.LogInformation("Action DELETE for /api/establishments returns 200");
            return Ok(new ResponseDetails() {Message = "Estabelecimento deletado com sucesso", StatusCode = 200});
        }
    }
}
