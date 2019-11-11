using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExtratosApi.Models;
using ExtratosApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExtratosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleasesController: ControllerBase
    {
        private readonly ReleasesService releasesService;

        public ReleasesController(ReleasesService releasesService) {
            this.releasesService = releasesService;
        }

        // GET api/releases
        [HttpGet]
        public async Task<ActionResult<List<Release>>> Get()
        {
            List<Release> releases;
            try {
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

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
