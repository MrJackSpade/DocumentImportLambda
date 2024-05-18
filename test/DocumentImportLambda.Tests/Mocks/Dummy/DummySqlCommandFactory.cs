using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Interfaces;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyDbCommandFactory : IDbCommandProvider
    {
        private readonly IDisposableCommand _command;

        public DummyDbCommandFactory(IDisposableCommand? command = null)
        {
            _command = command ?? new DummyDisposableCommand();
        }

        public IDbCommandProvider Clone(string? host, string? database)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public IDisposableCommand Request(string query)
        {
            return _command;
        }

        public void Return(DisposableSqlCommand disposableSqlCommand)
        {
        }

        public void TestConnection()
        {
        }
    }
}