using Amazon.Lambda.Core;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Database;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;
using System.Data.SqlClient;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    public class NonExecutingDbCommandFactory : AbstractDbCommandProvider
    {
        public NonExecutingDbCommandFactory(SqlAuthenticationType sqlAuthenticationType,
                                            string host,
                                            string database,
                                            ILambdaLogger logger,
                                            string username = "",
                                            string password = "",
                                            int port = 1433,
                                            bool trustCertificate = false) : base(sqlAuthenticationType, host, database, logger, username, password, port, trustCertificate)
        {
        }

        public override IDbCommandProvider Clone(string? host, string? database)
        {
            host = Ensure.NotNullOrEmpty(host);

            database = Ensure.NotNullOrEmpty(database);

            if (SqlAuthenticationType == SqlAuthenticationType.RdsToken)
            {
                return new NonExecutingDbCommandFactory(SqlAuthenticationType, this.Host, database, Logger, Username, Password, Port);
            }
            else
            {
                return new NonExecutingDbCommandFactory(SqlAuthenticationType, host, database, Logger, Username, Password, Port);
            }
        }

        public override IDisposableCommand Request(string query)
        {
            SqlConnection connection = GetConnection();

            return new NonExecutingSqlDatabaseCommand(connection);
        }

        public override void Return(DisposableSqlCommand disposableSqlCommand)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}