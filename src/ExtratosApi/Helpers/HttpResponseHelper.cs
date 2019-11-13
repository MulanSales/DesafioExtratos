using ExtratosApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExtratosApi.Helpers {
    public class HttpResponseHelper : ControllerBase
    {
        private readonly ResponseDetails responseDetails;
        public HttpResponseHelper()
        {
           this.responseDetails = new ResponseDetails(); 
        }
        public ObjectResult ErrorResponse(string message, int statusCode) {
            responseDetails.Message = message;
            responseDetails.StatusCode = statusCode;
            return StatusCode(statusCode, responseDetails);
        }
    }
}