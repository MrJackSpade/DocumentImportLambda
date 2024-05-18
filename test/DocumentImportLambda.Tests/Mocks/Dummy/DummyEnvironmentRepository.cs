using DocumentImportLambda.Database.Interfaces;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyEnvironmentRepository : IEnvironmentRepository
    {
        private readonly IDbCommandProvider _connectionFactory;

        public DummyEnvironmentRepository(IDbCommandProvider connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<IDbCommandProvider> GetClientCommandProvider(string clientCode)
        {
            return Task.FromResult(_connectionFactory);
        }
    }
}