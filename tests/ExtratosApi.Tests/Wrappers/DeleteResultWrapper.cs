using MongoDB.Driver;

namespace ExtratosApi.Tests.Wrappers
{
    public class DeleteResultWrapper : DeleteResult 
    {
        public DeleteResultWrapper() {}

        public override long DeletedCount => 1;

        public override bool IsAcknowledged => false;
    }
}