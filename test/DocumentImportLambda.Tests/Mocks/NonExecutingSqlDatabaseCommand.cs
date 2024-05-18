using DocumentImportLambda.Database.Extensions;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Tests.Mocks.Dummy;
using System.Data;
using System.Data.SqlClient;

namespace DocumentImportLambda.Tests.Mocks
{
    /// <summary>
    /// Its a database command in every way but non of the execute methods do anything.
    /// This makes it good for testing connections and such without modifying data
    /// </summary>
    internal class NonExecutingSqlDatabaseCommand : IDisposableCommand
    {
        private readonly SqlConnection _connection;

        private bool _disposedValue;

        public NonExecutingSqlDatabaseCommand(SqlConnection connection)
        {
            _connection = connection;
        }

        public string QueryString => string.Empty;

        public void AddParameter(string name, object? value)
        {
        }

        public void AddParameter(string name, SqlDbType type, object? value, int size = -1)
        {
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void ExecuteNonQuery()
        {
        }

        public IDataReader ExecuteReader()
        {
            return new DummyDataReader();
        }

        public T ExecuteScalar<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public async Task Open()
        {
            await _connection.TryOpen(10);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}