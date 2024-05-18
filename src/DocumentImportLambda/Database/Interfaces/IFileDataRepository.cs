using DocumentImportLambda.Database.Models;

namespace DocumentImportLambda.Database.Interfaces
{
    /// <summary>
    /// Defines and interface used to access a client FileData table
    /// </summary>
    public interface IFileDataRepository
    {
        /// <summary>
        /// Inserts a FileData into the client database represented by the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        Task Insert(IDbCommandProvider connectionString, FileData fileData);
    }
}