using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;
using System.Data;

namespace DocumentImportLambda.Database.Repositories
{
    /// <summary>
    /// Interfaces with a Control database, specifically the Environment table
    /// </summary>
    /// <param name="connectionFactory"></param>
    public class EnvironmentRepository(IDbCommandProvider connectionFactory) : IEnvironmentRepository
    {
        private const string SelectActiveEnvironmentQuery = "SELECT DatabaseName, DatabaseServer FROM [dbo].[Environment] WHERE EnvironmentLogin = @EnvironmentLogin AND Active = 1";

        private readonly Dictionary<string, IDbCommandProvider> _clientConnectionFactories = [];

        private readonly IDbCommandProvider _commandFactory = connectionFactory;

        /// <summary>
        /// Uses the Environment table of the Control database to find the connection string for a given client
        /// </summary>
        /// <param name="environmentLogin"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IDbCommandProvider> GetClientCommandProvider(string environmentLogin)
        {
            Ensure.NotNull(environmentLogin);

            //Check for cached string
            if (_clientConnectionFactories.TryGetValue(environmentLogin, out IDbCommandProvider? connectionFactory))
            {
                return connectionFactory;
            }

            using IDisposableCommand command = _commandFactory.Request(SelectActiveEnvironmentQuery);

            await command.Open();

            command.AddParameter("@EnvironmentLogin", environmentLogin);

            using IDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                string? databaseName = reader["DatabaseName"]?.ToString();
                string? databaseServer = reader["DatabaseServer"]?.ToString();

                connectionFactory = _commandFactory.Clone(databaseServer, databaseName);

                _clientConnectionFactories.Add(environmentLogin, connectionFactory);

                return connectionFactory;
            }
            else
            {
                throw new InvalidOperationException("Environment login not found or inactive.");
            }
        }
    }
}