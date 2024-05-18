using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Models;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyFileDataRepository : IFileDataRepository
    {
        public Task Insert(IDbCommandProvider connectionString, FileData fileData)
        {
            return Task.CompletedTask;
        }
    }
}