using DocumentImportLambda.Database.Extensions;
using DocumentImportLambda.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace DocumentImportLambda.Database.Utilities
{
    /// <summary>
    /// A reusable, disposable, abstracted SQL command that returns itself to a pool when disposed
    /// see <see cref="SqlCommandPool"/>
    /// </summary>
    /// <param name="sqlConnection"></param>
    /// <param name="pool"></param>
    public class DisposableSqlCommand(SqlConnection sqlConnection, SqlCommandPool pool) : IDisposableCommand
    {
        private readonly SqlCommandPool _pool = pool;

        private SqlCommand? _baseCommand;

        public string QueryString
        {
            get
            {
                if (_baseCommand is null)
                {
                    throw new NullReferenceException($"Can not access {nameof(_baseCommand)} if uninitialized {nameof(DisposableSqlCommand)}");
                }

                return _baseCommand.CommandText;
            }
        }

        public SqlConnection SqlConnection { get; } = sqlConnection;

        public void AddParameter(string name, object? value)
        {
            if (_baseCommand is null)
            {
                throw new NullReferenceException($"Can add parameters to uninitialized {nameof(DisposableSqlCommand)}");
            }

            _baseCommand.Parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
        }

        public void AddParameter(string name, SqlDbType type, object? value, int size = -1)
        {
            if (_baseCommand is null)
            {
                throw new NullReferenceException($"Can add parameters to uninitialized {nameof(DisposableSqlCommand)}");
            }

            var parameter = new SqlParameter(name, type, size)
            {
                Value = value ?? DBNull.Value
            };

            _baseCommand.Parameters.Add(parameter);
        }

        public void ClearQuery()
        {
            _baseCommand?.Dispose();
            _baseCommand = null;
        }

        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
        public void Dispose()
        {
            _pool.Return(this);
        }

        public void ExecuteNonQuery()
        {
            if (_baseCommand is null)
            {
                throw new NullReferenceException($"Can execute uninitialized {nameof(DisposableSqlCommand)}");
            }

            _baseCommand.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader()
        {
            if (_baseCommand is null)
            {
                throw new NullReferenceException($"Can execute uninitialized {nameof(DisposableSqlCommand)}");
            }

            return _baseCommand.ExecuteReader();
        }

        public T ExecuteScalar<T>()
        {
            if (_baseCommand is null)
            {
                throw new NullReferenceException($"Can execute uninitialized {nameof(DisposableSqlCommand)}");
            }

            return (T)_baseCommand.ExecuteScalar();
        }

        /// <summary>
        /// Attempts to open the database connection
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            await SqlConnection.TryOpen(10);
        }

        /// <summary>
        /// Sets the underlying SQL query to a new command
        /// </summary>
        /// <param name="query"></param>
        public void SetQuery(string query)
        {
            _baseCommand = new SqlCommand(query, SqlConnection);
        }
    }
}