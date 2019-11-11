using System;
using ExtratosApi.Models.Database;

namespace ExtratosApi.Models {

    public class Establishment : ICollectionSchema
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}