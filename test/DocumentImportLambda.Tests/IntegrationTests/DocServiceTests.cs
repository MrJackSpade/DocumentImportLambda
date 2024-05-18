using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Authentication.Services;
using DocumentImportLambda.Authentication.Settings;
using DocumentImportLambda.Authentication.Utilities;
using DocumentImportLambda.Aws.Dtos;
using DocumentImportLambda.Document.Dtos.Json.Request;
using DocumentImportLambda.Document.Services;
using DocumentImportLambda.Tests.Extensions;
using DocumentImportLambda.Tests.Mocks;
using DocumentImportLambda.Utilities;
using Microsoft.Extensions.Configuration;
using Xunit;

#if DEBUG
namespace DocumentImportLambda.Tests.IntegrationTests
{
    public class DocServiceTests
    {
        /// <summary>
        /// See README.md
        /// </summary>
        public static bool DisableTests => true;

        private const string TestTenant = "dc88a90f-3c6e-4d13-bac1-277dfcd369f1";

        public static AwsSecret MapFromConfiguration(IConfiguration configuration)
        {
            return new AwsSecret
            {
                DatabaseControl = configuration["database_control"],
                DatabaseHost = configuration["database_host"],
                DocServiceHost = configuration["docservice_rooturl"],
                OauthClientId = configuration["oauth_client_id"],
                OauthClientSecret = configuration["oauth_client_secret"],
                OauthHost = configuration["oauth_host"]
            };
        }


        [Fact]
        public async Task PostDocument()
        {
            if (DisableTests)
            {
                return;
            }

            // Load user secrets into configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddUserSecrets<AuthServiceTests>()
                .Build();

            // Deserialize the SecretModel from the user secrets
            AwsSecret secretModel = configuration.Deserialize<AwsSecret>();

            var authService = new AuthService(new OAuthServiceSettings(secretModel.OauthClientId, secretModel.OauthClientSecret, secretModel.OauthHost, secretModel.TenantHost), new DebugLogger(), new JsonClient(new DebugLogger()));

            string tenantToken = await authService.GetToken(Guid.Parse(TestTenant));

            var documentService = new DocumentService(Ensure.NotNull(secretModel.DocServiceHost), new DebugLogger(), new JsonClient(new DebugLogger()));

            string fileName = "Resources\\Yeah.pdf";

            string contentType = MimeTypeService.GetMimeType(fileName);

            byte[] data = File.ReadAllBytes(fileName);

            await documentService.Upload(new CreateDocumentRequest(Path.GetFileName(fileName), contentType, data), tenantToken);
        }
    }
}
#endif