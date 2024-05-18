using Amazon.Lambda.Core;
using DocumentImportLambda.Archive.Interfaces;
using DocumentImportLambda.Archive.Pocos;
using DocumentImportLambda.Archive.Services;
using DocumentImportLambda.Authentication.Exceptions;
using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Authentication.Services;
using DocumentImportLambda.Authentication.Settings;
using DocumentImportLambda.Authentication.Utilities;
using DocumentImportLambda.Aws.Constants;
using DocumentImportLambda.Aws.Dtos.Json;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Aws.Interfaces;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Aws.Utilities;
using DocumentImportLambda.Database.Extensions;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Database.Models;
using DocumentImportLambda.Database.Repositories;
using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Document.Constants;
using DocumentImportLambda.Document.Dtos.Json.Request;
using DocumentImportLambda.Document.Interfaces;
using DocumentImportLambda.Document.Services;
using DocumentImportLambda.Pocos;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Aws.Services
{
    /// <summary>
    /// This class performs the actual event processing. Its architected like this to ensure it functions on a single instance,
    /// helping with debugging and unit testing.
    /// </summary>
    public class LambdaEventHandler : IDisposable
    {
        private readonly LambdaEventArgs _args;

        private readonly bool _ownsArgs;

        private bool _disposedValue;

        /// <summary>
        /// This constructor isn't exposed because there is async required to properly init the components
        /// </summary>
        private LambdaEventHandler(LambdaEventArgs args, bool ownsArgs)
        {
            _args = args;
            _ownsArgs = ownsArgs;
        }

        /// <summary>
        /// Creates a new instance of the event handler while managing the async init tasks
        /// </summary>
        /// <returns></returns>
        public static async Task<LambdaEventHandler> Create(LambdaEventArgs args)
        {
            return await Task.FromResult(new LambdaEventHandler(args, false));
        }

        /// <summary>
        /// Creates a new instance of the event handler while managing the async init tasks
        /// </summary>
        /// <returns></returns>
        public static async Task<LambdaEventHandler> Create(ILambdaContext lambdaContext)
        {
            Ensure.NotNull(lambdaContext);

            HttpClient httpClient = new();

            IJsonClient jsonClient = new JsonClient(httpClient, lambdaContext.Logger);

            IReadEnvironmentVariables readEnvironmentVariables = new EnvironmentVariableService(lambdaContext.Logger);

            string region = readEnvironmentVariables.Read(EnvironmentVariables.Region);
            string secretName = readEnvironmentVariables.Read(EnvironmentVariables.SecretName);

            IReadSecrets readSecrets = new AwsSecretService(region, secretName, lambdaContext.Logger);

            var oAuthServiceSettings = new OAuthServiceSettings(
                await readSecrets.Read(SecretNames.ClientId),
                await readSecrets.Read(SecretNames.ClientSecret),
                await readSecrets.Read(SecretNames.OAuthHost),
                await readSecrets.Read(SecretNames.TenantHost)
            );

            string[] ignoreExtensions = [];

            if (await readSecrets.Read(SecretNames.IgnoreExtensions) is string ignoreExtensionsString)
            {
                ignoreExtensions = ignoreExtensionsString.Split(',').Select(s => s.Trim()).ToArray();
            }

            string documentHost = await readSecrets.Read(SecretNames.DocServiceRootUrl);

            IRetrieveAuthTokens retrieveAuthTokens = new AuthService(oAuthServiceSettings, lambdaContext.Logger, jsonClient);

            IDocumentService documentService = new DocumentService(documentHost, lambdaContext.Logger, jsonClient);

            IDbCommandProvider dbCommandProvider = new SqlCommandPool(
                        await readSecrets.Read<SqlAuthenticationType>(SecretNames.DatabaseAuthenticationType),
                        await readSecrets.Read(SecretNames.DatabaseHost),
                        await readSecrets.Read(SecretNames.DatabaseControl),
                        lambdaContext.Logger,
                        await readSecrets.Read(SecretNames.DatabaseUserName),
                        trustCertificate: bool.Parse(await readSecrets.Read(SecretNames.DatabaseTrustCertificate))
                        );

            IEnvironmentRepository environmentRepository = new EnvironmentRepository(dbCommandProvider);

            bool databaseTestBeforeUpload = bool.Parse(await readSecrets.Read(SecretNames.DatabaseTestBeforeUpload));

            IFileDataRepository fileDataRepository = new FileDataRepository(lambdaContext.Logger);

            IFileRepository fileRepository = new FileRepository(lambdaContext.Logger);

            IReadFileData readFileData = new S3BucketService(lambdaContext.Logger);

            IArchiveService archiveService = new ArchiveService("7z", "zip");

            return new LambdaEventHandler(new LambdaEventArgs(documentService,
                                                              environmentRepository,
                                                              fileDataRepository,
                                                              fileRepository,
                                                              lambdaContext,
                                                              readFileData,
                                                              retrieveAuthTokens,
                                                              archiveService,
                                                              dbCommandProvider,
                                                              httpClient,
                                                              jsonClient,
                                                              databaseTestBeforeUpload,
                                                              ignoreExtensions), true);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles the processing of an individual S3 event record by uploading the files and persisting the information
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingShortCodeException"></exception>
        public async Task Handle(S3EventNotificationRecord record)
        {
            Ensure.NotNull(record);

            try
            {
                EventValidation eventValidation = Validate(record, out string? messageFileName, out string? bucketName);

                if (eventValidation != EventValidation.Valid)
                {
                    return;
                }

                if (_args.DatabaseTestBeforeUpload)
                {
                    _args.DbCommandFactory.TestConnection();
                }

                //Build the definition for what we want to upload

                byte[] messageFileBytes = await _args.ReadFileData.ReadFileData(bucketName!, messageFileName!);

                var fileToProcess = new IncomingFileData(messageFileBytes, messageFileName!);

                var incomingFileKey = new ParsedFileKey(fileToProcess.FullName);

                string tenantShortCode = (await _args.RetrieveAuthTokens.GetShortCodeFromTenantIdAsync(incomingFileKey.TenantId)).SeaCode ?? throw new MissingShortCodeException(incomingFileKey.TenantId);

                //Prepare a database connection, tenant specific
                using IDbCommandProvider clientCommandFactory = await _args.EnvironmentRepository.GetClientCommandProvider(tenantShortCode);

                //Process the message
                ProcessFileContext processFileContext = new(clientCommandFactory, fileToProcess, tenantShortCode);

                //If required, test the database connection first
                if (_args.DatabaseTestBeforeUpload)
                {
                    processFileContext.ClientCommandFactory.TestConnection();
                }

                await ProcessFile(processFileContext);
            }
            catch (Exception ex)
            {
                _args.LambdaContext.Logger.LogError($"An exception occurred while importing the file key '{record?.S3?.Object?.Key}': {ex.Message}", ex);
                //Rethrow so that AWS knows the message needs to be moved to the DLQ
                throw;
            }
            finally
            {
                _args.LambdaContext.Logger.LogInformation("End processing message");
            }
        }

        public EventValidation Validate(S3EventNotificationRecord record, out string? messageFileName, out string? bucketName)
        {
            if (!record.Validate(out messageFileName, out bucketName, out List<string> errors))
            {
                _args.LambdaContext.Logger.LogErrors(errors);

                return EventValidation.Invalid;
            }

            if (_args.IgnoreExtensions.Contains(Path.GetExtension(messageFileName).Trim('.'), StringComparer.OrdinalIgnoreCase))
            {
                _args.LambdaContext.Logger.LogInformation($"Ignoring file {messageFileName} based on extension");

                return EventValidation.Skip;
            }

            return EventValidation.Valid;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_ownsArgs)
                    {
                        _args?.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Processes an archive of files by unzipping them and processing them individually
        /// </summary>
        /// <param name="processFileContext"></param>
        /// <param name="processFileKey"></param>
        /// <returns></returns>
        private async Task ProcessArchive(ProcessFileContext processFileContext)
        {
            var processFileKey = new ParsedFileKey(processFileContext.IncomingFileData.FullName);

            using IArchiveFile archiveFile = _args.ArchiveService.Open(processFileContext.IncomingFileData.Data, processFileContext.IncomingFileData.FullName);

            int files = 0;

            foreach (ArchiveFileData archiveFileData in archiveFile)
            {
                _args.LambdaContext.Logger.LogDebug($"Adding {archiveFileData.FullName} to queue");

                IncomingFileData fileToProcess = new(archiveFileData.GetBytes(), processFileKey.TenantId, archiveFileData.FullName);

                ProcessFileContext fileToProcessContext = new (processFileContext.ClientCommandFactory, fileToProcess, processFileContext.ShortCode);

                await ProcessFile(fileToProcessContext);

                archiveFileData.Dispose();

                _args.LambdaContext.Logger.LogDebug($"{++files} Files processed.");
            }
        }

        /// <summary>
        /// Processes an archive or non-archive, by directing it to the proper logic
        /// </summary>
        /// <param name="processFileContext"></param>
        /// <returns></returns>
        private async Task ProcessFile(ProcessFileContext processFileContext)
        {
            if (_args.ArchiveService.IsSupported(processFileContext.IncomingFileData.FullName))
            {
                _args.LambdaContext.Logger.LogDebug($"File {processFileContext.IncomingFileData.FullName} is a supported archive. Extracting...");

                await ProcessArchive(processFileContext);
            }
            else
            {
                _args.LambdaContext.Logger.LogDebug($"File {processFileContext.IncomingFileData} is regular file. No need to extract.");

                await ProcessSingleFile(processFileContext);
            }
        }


        /// <summary>
        /// Processes a non-archive file by uploading it to the document service
        /// </summary>
        /// <param name="processFileContext"></param>
        /// <param name="processFileKey"></param>
        /// <returns></returns>
        private async Task ProcessSingleFile(ProcessFileContext processFileContext)
        {
            var processFileKey = new ParsedFileKey(processFileContext.IncomingFileData.FullName);

            string targetContentType = MimeTypeService.GetMimeType(processFileContext.IncomingFileData.FullName);

            var loggingStopWatch = LoggingStopWatch.CreateStarted("Total Time", _args.LambdaContext.Logger);

            //Prepare document upload
            string authToken = await _args.RetrieveAuthTokens.GetToken(processFileKey.TenantId);

            var request = new CreateDocumentRequest(
                processFileKey.RelativeFilePath,
                targetContentType,
                processFileContext.IncomingFileData.Data
            );

            //Upload document
            Guid key = await _args.DocumentService.Upload(request, authToken);

            //Prepare database persist
            var fileRecord = new FileRecord(
                    processFileKey.RelativeFilePath,
                    targetContentType,
                    processFileContext.IncomingFileData.Data.Length,
                    MemberIds.Admin,
                    DateTime.Now
            );

            //Persist
            long fileId = await _args.FileRepository.Insert(processFileContext.ClientCommandFactory, fileRecord);

            var fileData = new FileData(fileId, (int)FileStorageMethodInds.DocumentService, false, key.ToString());

            await _args.FileDataRepository.Insert(processFileContext.ClientCommandFactory, fileData);

            loggingStopWatch.Stop();
        }
    }
}