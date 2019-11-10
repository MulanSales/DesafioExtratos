using System;

namespace ExtratosApi.Models.Database {
    public interface ICollectionSchema 
    {
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt{ get; set; }
    }
}