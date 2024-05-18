namespace DocumentImportLambda.Database.Interfaces
{
    /// <summary>
    /// Defines an interface used to retrieve a connection string to a given client database
    /// </summary>
    public interface IEnvironmentRepository
    {
        /// <summary>
        /// Gets a command provider for a client based on the clients short code
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        Task<IDbCommandProvider> GetClientCommandProvider(string clientCode);
    }
}