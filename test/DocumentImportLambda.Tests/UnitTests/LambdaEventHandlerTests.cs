using DocumentImportLambda.Archive.Services;
using DocumentImportLambda.Authentication.Utilities;
using DocumentImportLambda.Aws.Dtos.Json;
using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Aws.Services;
using DocumentImportLambda.Tests.Mocks;
using DocumentImportLambda.Tests.Mocks.Dummy;
using Xunit;

namespace DocumentImportLambda.Tests.UnitTests
{
    public class LambdaEventHandlerTests
    {
        [Fact]
        public async Task TestArchiveUpload()
        {
            var mockDocumentService = new DummyDocumentService();

            using LambdaEventHandler eventHandler = await LambdaEventHandler.Create(new LambdaEventArgs(
                mockDocumentService,
                new DummyEnvironmentRepository(
                    new DummyDbCommandFactory()),
                new DummyFileDataRepository(),
                new DummyFileRepository(),
                new DummyLambdaContext(),
                new ReadResourceFile("TextDoc.7z"),
                new DummyRetrieveAuthTokens(),
                new ArchiveService("7z"),
                new DummyDbCommandFactory(),
                new HttpClient(),
                new JsonClient(new DebugLogger()),
                false,
                Array.Empty<string>()));

            await eventHandler.Handle(new S3EventNotificationRecord("testBucket", "TextDoc.7z", Guid.Empty));

            Assert.True(mockDocumentService.HasUploaded("TextDoc.txt"));
            Assert.Equal(1, mockDocumentService.UploadedCount);
        }
    }
}