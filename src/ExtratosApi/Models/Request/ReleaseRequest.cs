using System;
using System.ComponentModel.DataAnnotations;

namespace ExtratosApi.Models.Request {

    public class ReleaseRequest
    {
        [Required]
        public string Date { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public string EstablishmentName { get; set; }

        [Required]
        public Decimal Amount { get; set; }

    }
}