using DocumentImportLambda.Interfaces;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    public class DummyDisposableCommand : IDisposableCommand
    {
        public string QueryString => string.Empty;

        public void AddParameter(string name, SqlDbType type, object? value, int size = -1)
        {
        }

        public void AddParameter(string name, object? value)
        {
        }

        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
        public void Dispose()
        {
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

        public Task Open()
        {
            return Task.CompletedTask;
        }
    }
}