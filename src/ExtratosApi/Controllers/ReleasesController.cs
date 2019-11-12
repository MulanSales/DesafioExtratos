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
    [Route("api/releases")]
    [ApiController]
    public class ReleasesController: ControllerBase
    {
        private readonly ILogger<ReleasesController> logger;
        private readonly ReleasesService releasesService;
        private readonly EstablishmentService establishmentService;
        public ReleasesController(ILogger<ReleasesController> logger, ReleasesService releasesService, EstablishmentService establishmentService) {
            this.logger = logger;
            this.releasesService = releasesService;
            this.establishmentService = establishmentService;
        }

        /// <summary>
        /// Returns an array of releases
        /// </summary>
        /// <returns>An array of all releases inserted in the past</returns>
        /// <response code="200">Returns the array</response>
        /// <response code="404">If can't find any release</response>    
        /// <response code="500">If an error in server side happens</response> 
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Release>>> Get()
        {
            List<Release> releases;
            try {
                logger.LogInformation("Trying to get releases from database");
                releases = await releasesService.GetAll();

                if (releases.Count == 0) {
                    var errorDetails = new ResponseDetails() {
                       Message = "Não foi possível encontrar nenhum Lançamento no banco de dados.",
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

            logger.LogInformation("Action GET for /api/releases returns 200");
            return Ok(releases);
        }

        /// <summary>
        /// Creates a new release
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/releases
        ///     {
        ///        "date": "05/05/2019",
        ///        "paymentMethod": "Credit",
        ///        "establishmentName": "Padaria Stn"
        ///        "amount": 34.88
        ///     }
        ///
        /// </remarks>
        /// <returns>The newly release created</returns>
        /// <response code="201">Returns the newly created release</response>
        /// <response code="400">If the request is not in correct format</response>    
        /// <response code="404">If the resource is not in the database</response>    
        /// <response code="500">If an error in server side happens</response>    
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Release>> Post([FromBody] ReleaseRequest body)
        {
            Release resultRelease;
            try {

                logger.LogInformation("Trying to get associated establishment");
                var establishment = await establishmentService.GetByName(body.EstablishmentName);

                if (establishment == null) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum estabelecimento associado com esse nome.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                logger.LogInformation("Inserting release into database");
                var newRelease = new Release() {
                    Date = body.Date,
                    PaymentMethod = body.PaymentMethod,
                    Amount = body.Amount,
                    EstablishmentName = establishment.Name,
                    CreatedAt = DateTime.Now
                };
                
                resultRelease = await releasesService.CreateItem(newRelease);

            } catch(Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            logger.LogInformation("Action POST for /api/releases returns 201");
            return Created("", resultRelease);
        }

        /// <summary>
        /// Updates an old release
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/releases/5dcaad2526235a471cfcccaf
        ///     {
        ///        "date": "06/05/2019",
        ///        "paymentMethod": "Credit",
        ///        "establishmentName": "Padaria Stn"
        ///        "amount": 56.88
        ///     }
        ///
        /// </remarks>
        /// <returns>The updated release</returns>
        /// <response code="200">Returns the updated release</response>
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
        public async Task<ActionResult<Release>> Put(string id, [FromBody] ReleaseRequest body)
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

            Release updatedRelease;
            try {
                logger.LogInformation("Trying to get a release with given id");
                var actualRelease = await releasesService.GetById(id);

                if (actualRelease == null) {
                     var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum lançamento associado com esse id.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                logger.LogInformation("Trying to get associated establishment");
                var establishment = await establishmentService.GetByName(body.EstablishmentName);

                if (establishment == null) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum estabelecimento associado com esse nome.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                updatedRelease = new Release() {
                    Id = id,
                    Date = body.Date,
                    PaymentMethod = body.PaymentMethod,
                    Amount = body.Amount,
                    EstablishmentName = establishment.Name,
                    CreatedAt = actualRelease.CreatedAt,
                    UpdatedAt = DateTime.Now
                };

                logger.LogInformation("Trying to update release with id: " + id);
                var replaceResult = await releasesService.UpdateById(id, updatedRelease);

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

            return Ok(updatedRelease);
        }

        /// <summary>
        /// Deletes a release
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/releases/5dcaad2526235a471cfcccaf
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
                logger.LogInformation("Trying to get a release with given id");
                var actualRelease = await releasesService.GetById(id);

                if (actualRelease == null) {
                     var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível encontrar nenhum lançamento associado com esse id.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                var deleteResult = await releasesService.RemoveById(id);
                if(!deleteResult.IsAcknowledged) {
                    var errorDetails = new ResponseDetails() {
                        Message = "Não foi possível realizar a atualização seguindo os valores passados.",
                        StatusCode = 406
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return StatusCode(406, errorDetails);
                }

            } catch(Exception ex) {
                logger.LogInformation("Exception: " + ex.Message);
                throw ex;
            }

            return Ok(new ResponseDetails() {Message = "Lançamento deletado com sucesso", StatusCode = 200});
        }
    }
}
