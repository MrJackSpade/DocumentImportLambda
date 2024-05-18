using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Models;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyFileRepository : IFileRepository
    {
        private readonly long _id;

        public DummyFileRepository(long id = 0)
        {
            _id = id;
        }

        public Task<long> Insert(IDbCommandProvider connectionString, FileRecord record)
        {
            return Task.FromResult(_id);
        }
    }
}