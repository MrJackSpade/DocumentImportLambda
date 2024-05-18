using Amazon.Lambda.Core;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Models;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;
using System.Data;

namespace DocumentImportLambda.Database.Repositories
{
    /// <summary>
    /// This repository interfaces the process with the client FileData table used by SEA/RQS
    /// for storing the record representing the file uploaded to the document service
    /// </summary>
    public class FileDataRepository(ILambdaLogger logger) : IFileDataRepository
    {
        private const string FileDataInsertQuery = "INSERT INTO control.FileData (FileDataId, FileStorageMethodInd, StorageKey, FileContents, IsEncrypted, IsCompressed, Salt, IV) VALUES (@FileDataId, @FileStorageMethodInd, @StorageKey, @FileContents, @IsEncrypted, @IsCompressed, @Salt, @IV)";

        private readonly ILambdaLogger _logger = logger;

        /// <summary>
        /// Inserts the provided file data into the file table
        /// </summary>
        /// <param name="connectionFactory"></param>
        /// <param name="fileData"></param>
        public async Task Insert(IDbCommandProvider connectionFactory, FileData fileData)
        {
            _logger.LogInformation($"Inserting fileData for fileDataId: {fileData.FileDataId}");

            Ensure.NotNull(connectionFactory);
            Ensure.GreaterThanZero(fileData.FileDataId);
            Ensure.GreaterThanZero(fileData.FileStorageMethodInd);

            using IDisposableCommand command = connectionFactory.Request(FileDataInsertQuery);

            await command.Open();

            // Adding parameters to prevent SQL injection
            command.AddParameter("@FileDataId", fileData.FileDataId);
            command.AddParameter("@FileStorageMethodInd", fileData.FileStorageMethodInd);
            command.AddParameter("@StorageKey", fileData.StorageKey);
            command.AddParameter("@IsEncrypted", fileData.IsEncrypted);
            command.AddParameter("@IsCompressed", fileData.IsCompressed);
            command.AddParameter("@FileContents", SqlDbType.VarBinary, fileData.FileContents);
            command.AddParameter("@IV", SqlDbType.VarBinary, fileData.IV);
            command.AddParameter("@Salt", SqlDbType.UniqueIdentifier, fileData.Salt);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred while executing the command '{command.QueryString}'", ex);
                throw;
            }

            _logger.LogInformation($"Inserted fileData for fileDataId: {fileData.FileDataId}");
        }
    }
}