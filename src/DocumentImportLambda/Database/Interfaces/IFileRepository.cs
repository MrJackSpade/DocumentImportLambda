using DocumentImportLambda.Database.Models;

namespace DocumentImportLambda.Database.Interfaces
{
    /// <summary>
    /// Defines an interface used to access the client File table
    /// </summary>
    public interface IFileRepository
    {
        /// <summary>
        /// Inserts a file record into the client database using the provided connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        Task<long> Insert(IDbCommandProvider connectionString, FileRecord record);
    }
}