using DocumentImportLambda.Authentication.Dtos.Json.Response;
using DocumentImportLambda.Authentication.Services;
using DocumentImportLambda.Authentication.Settings;
using DocumentImportLambda.Authentication.Utilities;
using DocumentImportLambda.Aws.Dtos;
using DocumentImportLambda.Tests.Extensions;
using DocumentImportLambda.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Xunit;

#if DEBUG
namespace DocumentImportLambda.Tests.IntegrationTests
{
    public class AuthServiceTests
    {
        private const string TestTenant = "dc88a90f-3c6e-4d13-bac1-277dfcd369f1";

        /// <summary>
        /// See README.md
        /// </summary>
        public static bool DisableTests => true;

        [Fact]
        public async Task GetRootToken()
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

            var authService = new AuthService(new OAuthServiceSettings(secretModel.OauthClientId,
                                                                       secretModel.OauthClientSecret,
                                                                       secretModel.OauthHost,
                                                                       secretModel.TenantHost), new DebugLogger(), new JsonClient(new DebugLogger()));

            RootAuthToken rootToken = await authService.GetRootToken();

            Assert.NotNull(rootToken.AccessToken);
        }

        [Fact]
        public async Task GetShortCode()
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

            var authService = new AuthService(new OAuthServiceSettings(secretModel.OauthClientId,
                                                                       secretModel.OauthClientSecret,
                                                                       secretModel.OauthHost,
                                                                       secretModel.TenantHost), new DebugLogger(), new JsonClient(new DebugLogger()));

            TenantApiResponse tenantApiResponse = await authService.GetShortCodeFromTenantIdAsync(Guid.Parse(TestTenant));

            Assert.False(string.IsNullOrWhiteSpace(tenantApiResponse.SeaCode));

        }


        [Fact]
        public async Task GetTenantToken()
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

            var authService = new AuthService(new OAuthServiceSettings(secretModel.OauthClientId,
                                                                       secretModel.OauthClientSecret,
                                                                       secretModel.OauthHost,
                                                                       secretModel.TenantHost), new DebugLogger(), new JsonClient(new DebugLogger()));

            string tenantToken = await authService.GetToken(Guid.Parse(TestTenant));

            Assert.False(string.IsNullOrWhiteSpace(tenantToken));
        }
    }
}
#endif