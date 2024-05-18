using Amazon.Lambda.Core;
using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Document.Dtos.Json.Request;
using DocumentImportLambda.Document.Dtos.Json.Response;
using DocumentImportLambda.Document.Exceptions;
using DocumentImportLambda.Document.Interfaces;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Document.Services
{
    /// <summary>
    /// Interfaces with the Document service to upload document binaries, and return the key that will be inserted into
    /// the client database
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly string _documentsUrl;

        private readonly IJsonClient _jsonClient;

        private readonly ILambdaLogger _logger;

        private readonly string _rootUrl;

        public DocumentService(string host, ILambdaLogger logger, IJsonClient jsonClient)
        {
            _rootUrl = Ensure.NotNullOrWhiteSpace(host);
            _logger = Ensure.NotNull(logger);
            _jsonClient = Ensure.NotNull(jsonClient);

            _documentsUrl = $"https://{_rootUrl}/documents/";

            _logger.LogDebug("Initializing Document Service");
        }

        /// <summary>
        /// Uploads a document by calling the DocumentService to get a presigned link, posting the data at that location,
        /// and returns the GUID to be used in the SEA database.
        /// </summary>
        /// <param name="upload">The document upload request containing the necessary information.</param>
        /// <param name="authToken">The authentication token to be used for the API requests.</param>
        /// <returns>The GUID of the uploaded document.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="upload"/> or <paramref name="authToken"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="authToken"/> is a null or whitespace string.</exception>
        /// <exception cref="CreateDocumentException">Thrown when the POST request to create the document fails.</exception>
        /// <exception cref="UploadDocumentException">Thrown when the PUT request to upload the document data fails.</exception>
        public async Task<Guid> Upload(CreateDocumentRequest upload, string authToken)
        {
            Ensure.NotNull(upload);
            Ensure.NotNullOrWhiteSpace(authToken);

            CreateDocumentResponse? response = await CreateDocument(upload, authToken);

            DocumentVersionResponse responseVersion = response.Versions[0];

            await PutData(upload, responseVersion);

            _logger.LogInformation($"New Document ID: {response.DocumentId}");

            return response.DocumentId;
        }

        private async Task<CreateDocumentResponse> CreateDocument(CreateDocumentRequest upload, string authToken)
        {
            Ensure.NotNull(upload);
            Ensure.NotNullOrWhiteSpace(authToken);

            string? fileName = upload.Versions[0].FileName;

            try
            {
                _logger.LogInformation($"Creating document {fileName}...");

                return await _jsonClient.CreateAsync<CreateDocumentResponse>(_documentsUrl, upload, authToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create document {fileName}", ex);
                throw;
            }
        }

        private async Task PutData(CreateDocumentRequest upload, DocumentVersionResponse responseVersion)
        {
            Ensure.NotNull(upload);
            Ensure.NotNull(responseVersion);

            string? fileName = upload.Versions[0].FileName;

            try
            {
                _logger.LogInformation($"Creating document {fileName}...");

                await _jsonClient.PutAsync(responseVersion.Link.Url, responseVersion.ContentType, upload.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create document {fileName}", ex);
                throw;
            }
        }
    }
}