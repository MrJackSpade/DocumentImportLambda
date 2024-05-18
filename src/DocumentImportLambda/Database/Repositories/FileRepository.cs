using Amazon.Lambda.Core;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Models;
using DocumentImportLambda.Interfaces;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Database.Repositories
{
    /// <summary>
    /// This repository interfaces the process with the client File table used by SEA/RQS
    /// for storing the record representing the file uploaded to the document service
    /// </summary>
    public class FileRepository(ILambdaLogger logger) : IFileRepository
    {
        private const string InsertFileQuery = "INSERT INTO control.[File] (Description, Type, Size, fxEnteredById, EnteredDate, StatusInd) VALUES (@Description, @Type, @Size, @FxEnteredById, @EnteredDate, @StatusInd); SELECT SCOPE_IDENTITY();";

        private readonly ILambdaLogger _logger = logger;

        public async Task<long> Insert(IDbCommandProvider databaseCommand, FileRecord record)
        {
            _logger.LogInformation($"Inserting file for fileName: {record.Description}");

            //Validation is implementation specific meaning subsequent coalesce is
            //redundant however for safety sake I'm leaving it.
            Ensure.NotNull(databaseCommand);
            Ensure.NotNull(record.Type);
            Ensure.NotDefault(record.EnteredDate);
            Ensure.GreaterThanZero(record.Size);

            using IDisposableCommand command = databaseCommand.Request(InsertFileQuery);

            await command.Open();

            // Adding parameters to prevent SQL injection
            command.AddParameter("@Description", record.Description);
            command.AddParameter("@Type", record.Type);
            command.AddParameter("@Size", record.Size);
            command.AddParameter("@FxEnteredById", record.FxEnteredById);
            command.AddParameter("@EnteredDate", record.EnteredDate);
            command.AddParameter("@StatusInd", record.StatusInd);

            // ExecuteScalar used to retrieve the first column of the first row in the result set
            // Cast the result to long since FileId is a bigint
            long result = 0;

            try
            {
                result = Convert.ToInt64(command.ExecuteScalar<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred while executing the command '{command.QueryString}'", ex);
                throw;
            }

            _logger.LogInformation($"Inserted file for fileName: {record.Description}, Id: {result}");

            return result;
        }
    }
}