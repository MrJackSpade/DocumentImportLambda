using DocumentImportLambda.Authentication.Utilities;
using DocumentImportLambda.Aws.Dtos;
using DocumentImportLambda.Aws.Dtos.Json;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Aws.Services;
using DocumentImportLambda.Database.Models;
using DocumentImportLambda.Database.Repositories;
using DocumentImportLambda.Database.Utilities;
using DocumentImportLambda.Document.Constants;
using DocumentImportLambda.Document.Services;
using DocumentImportLambda.Tests.Extensions;
using DocumentImportLambda.Tests.Mocks;
using DocumentImportLambda.Tests.Mocks.Dummy;
using DocumentImportLambda.Utilities;
using Microsoft.Extensions.Configuration;
using Xunit;

#if DEBUG
namespace DocumentImportLambda.Tests.IntegrationTests
{
    public class DatabaseFileTests
    {
        public const string TEST_CLIENT = "testqa";

        /// <summary>
        /// See README.md
        /// </summary>
        public static bool DisableTests => true;

        [Fact]
        public async Task TestBulkInsert()
        {
            if (DisableTests)
            {
                return;
            }

            var mockDocumentService = new DummyDocumentService();

            var _fileRepository = new FileRepository(new DebugLogger());

            var _fileDataRepository = new FileDataRepository(new DebugLogger());

            AwsSecret secretModel = GetSecrets();

            using var sqlConnectionFactory = new SqlCommandPool(secretModel.DatabaseAuthenticationType,
                                                            Ensure.NotNull(secretModel.DatabaseHost),
                                                            Ensure.NotNull(secretModel.DatabaseControl),
                                                            new DebugLogger(),
                                                            secretModel.DatabaseUserName ?? string.Empty,
                                                            trustCertificate: secretModel.DatabaseTrustCertificate);

            var environmentRepository = new EnvironmentRepository(sqlConnectionFactory);

            using LambdaEventHandler eventHandler = await LambdaEventHandler.Create(new LambdaEventArgs(
                mockDocumentService,
                environmentRepository,
                _fileDataRepository,
                _fileRepository,
                new DummyLambdaContext(),
                new DummyFileReader(),
                new DummyRetrieveAuthTokens("autotestqa"),
                new BulkArchiveFileProvider(50_000_000, 128, "pdf", "zip"),
                new DummyDbCommandFactory(),
                new HttpClient(),
                new JsonClient(new DebugLogger()),
                false,
                Array.Empty<string>()));

            await eventHandler.Handle(new S3EventNotificationRecord("testBucket", "BulkFile.zip", Guid.Empty));

            Assert.True(mockDocumentService.HasUploaded("TextDoc.txt"));
            Assert.Equal(1, mockDocumentService.UploadedCount);

        }

        [Fact]
        public async Task TestInsert()
        {
            if (DisableTests)
            {
                return;
            }

            var _fileRepository = new FileRepository(new DebugLogger());

            var _fileDataRepository = new FileDataRepository(new DebugLogger());

            AwsSecret secretModel = GetSecrets();

            using var sqlConnectionFactory = new SqlCommandPool(secretModel.DatabaseAuthenticationType,
                                                            Ensure.NotNull(secretModel.DatabaseHost),
                                                            Ensure.NotNull(secretModel.DatabaseControl),
                                                            new DebugLogger(),
                                                            secretModel.DatabaseUserName ?? string.Empty,
                                                            trustCertificate: secretModel.DatabaseTrustCertificate);

            string fileName = Guid.NewGuid().ToString() + ".pdf";

            var fileRecord = new FileRecord(fileName, MimeTypeService.GetMimeType(fileName), 666, MemberIds.Admin, DateTime.Now);

            //Persist
            long fileId = await _fileRepository.Insert(sqlConnectionFactory, fileRecord);

            var fileData = new FileData(fileId, (int)FileStorageMethodInds.DocumentService, false, Guid.NewGuid().ToString());

            await _fileDataRepository.Insert(sqlConnectionFactory, fileData);
        }


        private static AwsSecret GetSecrets()
        {
            // Load user secrets into configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddUserSecrets<AuthServiceTests>()
                .Build();

            // Deserialize the SecretModel from the user secrets
            AwsSecret secretModel = configuration.Deserialize<AwsSecret>();
            return secretModel;
        }
    }
}
#endif