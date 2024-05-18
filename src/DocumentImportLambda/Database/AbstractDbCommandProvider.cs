using Amazon.Lambda.Core;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;
using System.Data.SqlClient;

namespace DocumentImportLambda.Database
{
    public abstract class AbstractDbCommandProvider(SqlAuthenticationType sqlAuthenticationType,
                                   string host,
                                   string database,
                                   ILambdaLogger logger,
                                   string username = "",
                                   string password = "",
                                   int port = 1433,
                                   bool trustCertificate = false) : IDbCommandProvider
    {
        private readonly string _database = Ensure.NotNullOrWhiteSpace(database);

        private readonly bool _trustCertificate = trustCertificate;

        public string ConnectionString
        {
            get
            {
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = $"tcp:{Host},{Port}",
                    InitialCatalog = _database,
                    TrustServerCertificate = _trustCertificate
                };

                switch (SqlAuthenticationType)
                {
                    case SqlAuthenticationType.Integrated:
                        sqlConnectionStringBuilder.IntegratedSecurity = true;
                        break;

                    case SqlAuthenticationType.RdsToken:
                        sqlConnectionStringBuilder.Pooling = false;
                        sqlConnectionStringBuilder.MultiSubnetFailover = true;
                        sqlConnectionStringBuilder.PersistSecurityInfo = false;
                        sqlConnectionStringBuilder.ConnectTimeout = 30;
                        sqlConnectionStringBuilder.Encrypt = true;
                        break;

                    default: throw new NotImplementedException();
                }

                return sqlConnectionStringBuilder.ConnectionString;
            }
        }

        protected string Host { get; } = Ensure.NotNullOrWhiteSpace(host);

        protected ILambdaLogger Logger { get; } = logger;

        protected string Password { get; } = Ensure.NotNull(password);

        protected int Port { get; } = Ensure.NotDefault(port);

        protected SqlAuthenticationType SqlAuthenticationType { get; } = sqlAuthenticationType;

        protected string Username { get; } = username;

        public abstract IDbCommandProvider Clone(string? host, string? database);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public abstract IDisposableCommand Request(string query);

        public abstract void Return(DisposableSqlCommand disposableSqlCommand);

        public virtual void TestConnection()
        {
            SqlConnection connection = GetConnection();

            connection.Open();

            connection.Close();

            connection.Dispose();
        }

        protected abstract void Dispose(bool disposing);

        protected SqlConnection GetConnection()
        {
            // Create Connection String to connect to your RDS for SQL Server instance via RDS Proxy

            SqlConnection connection;

            Logger.LogDebug($"Building '{SqlAuthenticationType}' connection to database '{_database}' on host '{Host}'");

            switch (SqlAuthenticationType)
            {
                case SqlAuthenticationType.Integrated:
                    connection = new SqlConnection(ConnectionString);
                    break;

                case SqlAuthenticationType.RdsToken:
                    // Create DB Authentication Token
                    string authToken = Amazon.RDS.Util.RDSAuthTokenGenerator.GenerateAuthToken(Host, Port, Username);

                    // Create a new SQLConnection Object by using ConnectionString created in previous steps
                    connection = new SqlConnection(ConnectionString)
                    {
                        // Use AccessToken Property and provide DB Authentication Token to authenticate connection with RDS for SQL Server instance
                        AccessToken = authToken
                    };

                    break;

                default: throw new NotImplementedException();
            }

            return connection;
        }
    }
}