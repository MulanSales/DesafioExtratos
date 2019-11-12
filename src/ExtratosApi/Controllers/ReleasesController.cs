using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ExtratosApi.Models;
using ExtratosApi.Models.Request;
using ExtratosApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        // GET api/releases
        [HttpGet]
        public async Task<ActionResult<List<Release>>> Get()
        {
            List<Release> releases;
            try {
                logger.LogInformation("Trying to get releases from database");
                releases = await releasesService.GetAll();

                if (releases.Count == 0) {
                    var errorDetails = new ErrorDetails() {
                       Message = "Não foi possível encontrar nenhum Lançamento no banco de dados.",
                       StatusCode = 404
                   };
                  return NotFound(errorDetails);
               }
            } 
            catch (Exception ex) {
                throw ex;
            }

            return Ok(releases);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public void Get(int id)
        {
        }

        /// <summary>
        /// Creates a new release
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/releases/
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
        /// <response code="400">If the item is null</response>    
        /// <response code="500">If an error in server side happens</response>    
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Release>> Post([FromBody] ReleaseRequest body)
        {
            Release resultRelease;
            try {

                logger.LogInformation("Trying to get associated establishment");
                var establishment = await establishmentService.GetByName(body.EstablishmentName);

                if (establishment == null) {
                    var errorDetails = new ErrorDetails() {
                        Message = "Não foi possível encontrar nenhum estabelecimento associado com esse nome.",
                        StatusCode = 404
                    };
                    logger.LogInformation("Error: " + errorDetails.Message);
                    return NotFound(errorDetails);
                }

                logger.LogInformation("Inserting release into database");
                var newRelease = new Release() {
                    Date = DateTime.ParseExact(body.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    PaymentMethod = body.PaymentMethod,
                    Amount = body.Amount,
                    EstablishmentName = establishment.Name,
                    CreatedAt = DateTime.Now
                };
                
                resultRelease = await releasesService.CreateItem(newRelease);

            } catch(Exception ex) {
                throw ex;
            }

            logger.LogInformation("Release created with success and returned");
            return Created("", resultRelease);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
