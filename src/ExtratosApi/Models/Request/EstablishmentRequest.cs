using System;
using System.ComponentModel.DataAnnotations;

namespace ExtratosApi.Models.Request {

    public class EstablishmentRequest 
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

    }
}