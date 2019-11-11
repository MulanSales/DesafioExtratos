using System;
using ExtratosApi.Models.Database;

namespace ExtratosApi.Models {

    public class Release : ICollectionSchema
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public Establishment Establishment { get; set; }

        public Double Amount { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

}
