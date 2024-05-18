using Amazon.Lambda.Core;
using Amazon.Runtime.CredentialManagement;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data.SqlClient;

namespace DocumentImportLambda.Database.Utilities
{
    /// <summary>
    /// Creates a new instance of the Sql Command Pool. The RDS proxy has a very low throughput for authentication, requiring connection pooling.
    /// Unfortunately connection pooling itself causes a lot of problems as a result of how the MS database interfaces are architected, making
    /// that effectively impossible. In order to compensate, we need a new abstraction for a command that can hide the connection type, making
    /// the command level pooling method the more viable option.
    /// </summary>
    /// <param name="sqlAuthenticationType">The underlying authentication type</param>
    /// <param name="host">The host containing the databases that will be interacted with</param>
    /// <param name="database">The database this command pool will target</param>
    /// <param name="logger"></param>
    /// <param name="username">Username if Sql Credentials</param>
    /// <param name="password">Password if Sql Credentials</param>
    /// <param name="port"></param>
    /// <param name="trustCertificate">Trust certificate for testing only</param>
    public class SqlCommandPool(SqlAuthenticationType sqlAuthenticationType,
                          string host,
                          string database,
                          ILambdaLogger logger,
                          string username = "",
                          string password = "",
                          int port = 1433,
                          bool trustCertificate = false) : AbstractDbCommandProvider(sqlAuthenticationType, host, database, logger, username, password, port, trustCertificate), IDisposable
    {
        /// <summary>
        /// Pools commands for re-use. Each command maintains its own connection so this is effectively a connection pool.
        /// <see cref="SqlCommandPool"/> for more information
        /// </summary>
        private readonly Dictionary<string, Queue<DisposableSqlCommand>> _commandPool = [];

        private readonly object _lock = new();

        private bool _disposedValue;

        /// <summary>
        /// Copies all of the existing settings to a new host and database, for switching from
        /// environment to client easily without a lot of reconfiguring
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public override IDbCommandProvider Clone(string? host, string? database)
        {
            host = Ensure.NotNullOrEmpty(host);
            database = Ensure.NotNullOrEmpty(database);

            if (SqlAuthenticationType == SqlAuthenticationType.RdsToken)
            {
                Logger.LogDebug($"Authentication kind {SqlAuthenticationType.RdsToken}, using proxy to clone connection information...");
                return new SqlCommandPool(SqlAuthenticationType, Host, database, Logger, Username, Password, Port);
            }
            else
            {
                return new SqlCommandPool(SqlAuthenticationType, host, database, Logger, Username, Password, Port);
            }
        }

        /// <summary>
        /// Requests a new pooled command with the provided query text
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IDisposableCommand Request(string query)
        {
            lock (_lock)
            {
                string key = ConnectionString;

                Queue<DisposableSqlCommand>? connectionPool = GetOrCreatePool(key);

                //If we have no existing command, create a new one
                if (!connectionPool.TryDequeue(out DisposableSqlCommand? toReturn))
                {
                    Logger.LogDebug($"Creating new connection to {key}");

                    toReturn = new DisposableSqlCommand(GetConnection(), this);
                }
                else
                {
                    Logger.LogDebug($"Using existing connection to {key}");
                }

                Logger.LogDebug($"{connectionPool.Count + 1} active connection to {key}");

                toReturn.SetQuery(query);

                return toReturn;
            }
        }

        public override void Return(DisposableSqlCommand disposableSqlCommand)
        {
            lock (_lock)
            {
                string key = disposableSqlCommand.SqlConnection.ConnectionString;

                Logger.LogDebug($"Returning connection for {key} to pool");

                disposableSqlCommand.ClearQuery();

                GetOrCreatePool(key)
                    .Enqueue(disposableSqlCommand);
            }
        }

        /// <summary>
        /// Closes all of the pooled connections
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue && disposing)
            {
                lock (_lock)
                {
                    foreach (Queue<DisposableSqlCommand> connectionPool in _commandPool.Values)
                    {
                        while (connectionPool.TryDequeue(out DisposableSqlCommand? command))
                        {
                            try
                            {
                                command.ClearQuery();
                                command.SqlConnection.Close();
                                command.SqlConnection.Dispose();
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError($"Exception disposing connection to {command.SqlConnection.ConnectionString}", ex);
                            }
                        }
                    }

                    _commandPool.Clear();
                }
            }

            _disposedValue = true;
        }

        /// <summary>
        /// Gets a pool of connections for the provided connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Queue<DisposableSqlCommand> GetOrCreatePool(string connectionString)
        {
            //Each pool is connection string specific since its really the database connections we're pooling
            //so each connection string has its own pool.
            if (!_commandPool.TryGetValue(connectionString, out Queue<DisposableSqlCommand>? connectionPool))
            {
                Logger.LogDebug($"Creating new connection pool for {connectionString}");
                connectionPool = new Queue<DisposableSqlCommand>();
                _commandPool.Add(connectionString, connectionPool);
            }

            return connectionPool;
        }
    }
}