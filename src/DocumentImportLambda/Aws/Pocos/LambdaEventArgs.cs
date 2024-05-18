using Amazon.Lambda.Core;
using DocumentImportLambda.Archive.Interfaces;
using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Aws.Interfaces;
using DocumentImportLambda.Database.Interfaces;
using DocumentImportLambda.Document.Interfaces;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Aws.Pocos
{
    public class LambdaEventArgs(IDocumentService documentService,
                           IEnvironmentRepository environmentRepository,
                           IFileDataRepository fileDataRepository,
                           IFileRepository fileRepository,
                           ILambdaContext lambdaContext,
                           IReadFileData readFileData,
                           IRetrieveAuthTokens retrieveAuthTokens,
                           IArchiveService archiveService,
                           IDbCommandProvider dbCommandProvider,
                           HttpClient httpClient,
                           IJsonClient jsonClient,
                           bool databaseTestBeforeUpload,
                           string[] ignoreExtensions) : IDisposable
    {
        private bool _disposedValue;

        public IArchiveService ArchiveService { get; private set; } = Ensure.NotNull(archiveService);

        public bool DatabaseTestBeforeUpload { get; private set; } = databaseTestBeforeUpload;

        public IDbCommandProvider DbCommandFactory { get; private set; } = Ensure.NotNull(dbCommandProvider);

        public IDocumentService DocumentService { get; private set; } = Ensure.NotNull(documentService);

        public IEnvironmentRepository EnvironmentRepository { get; private set; } = Ensure.NotNull(environmentRepository);

        public IFileDataRepository FileDataRepository { get; private set; } = Ensure.NotNull(fileDataRepository);

        public IFileRepository FileRepository { get; private set; } = Ensure.NotNull(fileRepository);

        public HttpClient HttpClient { get; private set; } = Ensure.NotNull(httpClient);

        public string[] IgnoreExtensions { get; private set; } = Ensure.NotNull(ignoreExtensions);

        public IJsonClient JsonClient { get; private set; } = Ensure.NotNull(jsonClient);

        public ILambdaContext LambdaContext { get; private set; } = Ensure.NotNull(lambdaContext);

        public IReadFileData ReadFileData { get; private set; } = Ensure.NotNull(readFileData);

        public IRetrieveAuthTokens RetrieveAuthTokens { get; private set; } = Ensure.NotNull(retrieveAuthTokens);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    DbCommandFactory?.Dispose();
                    HttpClient?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}