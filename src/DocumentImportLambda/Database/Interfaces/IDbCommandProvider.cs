using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Interfaces;

namespace DocumentImportLambda.Database.Interfaces
{
    /// <summary>
    /// This only exists because the SqlConnection needs to be abstracted for unit tests, but SqlCommand requires a concrete type of
    /// SqlConnection, so the abstraction needs to go all the way down. See <see cref="SqlCommandPool"/>
    /// </summary>
    public interface IDbCommandProvider : IDisposable
    {
        /// <summary>
        /// Clones a connection using the provided host and database
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        IDbCommandProvider Clone(string? host, string? database);

        /// <summary>
        /// Requests a new executable DbCommand
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IDisposableCommand Request(string query);

        /// <summary>
        /// Tests the connection to the database
        /// </summary>
        void TestConnection();
    }
}