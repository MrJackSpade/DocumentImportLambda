using Amazon.Lambda.Core;
using DocumentImportLambda.Authentication.Constants;
using DocumentImportLambda.Authentication.Dtos.Json.Response;
using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Authentication.Settings;
using DocumentImportLambda.Aws.Extensions;
using DocumentImportLambda.Utilities;
using System.Collections.Concurrent;

namespace DocumentImportLambda.Authentication.Services
{
    /// <summary>
    /// Manages authentication tokens used to access the document service
    /// </summary>
    public class AuthService : IRetrieveAuthTokens
    {
        /// <summary>
        /// A collection of authentication tokens based on tenant-id to reduce round trips to the auth service
        /// </summary>
        private readonly Dictionary<Guid, string> _authTokens = [];

        private readonly IJsonClient _jsonClient;

        private readonly ILambdaLogger _logger;

        private readonly OAuthServiceSettings _settings;

        /// <summary>
        /// A collection of tenant short-codes based on tenant-id to reduce round trips to the auth service
        /// </summary>
        private readonly ConcurrentDictionary<Guid, TenantApiResponse> _shortCodeCache = new();

        /// <summary>
        /// Root authentication token that is used to look up tenant information
        /// </summary>
        private RootAuthToken? _rootToken;

        public AuthService(OAuthServiceSettings settings, ILambdaLogger logger, IJsonClient jsonClient)
        {
            _settings = Ensure.NotNull(settings);
            _logger = Ensure.NotNull(logger);
            _jsonClient = Ensure.NotNull(jsonClient);

            _logger.LogDebug("Initializing Auth Service");
        }

        /// <summary>
        /// Gets the root authentication token which is later used to retrieve the client
        /// specific token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<RootAuthToken> GetRootToken()
        {
            try
            {
                _logger.LogInformation("Getting root token...");

                return await _jsonClient.PostAsync<RootAuthToken>(
                    $"https://{_settings.TokenHost}/oauth2/token",
                    ReadSettingsAsDictionary()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("An exception occurred while getting root token", ex);
                throw;
            }
        }

        /// <summary>
        /// Uses the TenantId to retrieve the Environment Code that will be used to
        /// determine the client database, the target for the file insert
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<TenantApiResponse> GetShortCodeFromTenantIdAsync(Guid tenantId)
        {
            Ensure.NotDefault(tenantId);

            _rootToken ??= await GetRootToken();

            return await GetShortCodeFromTenantIdAsync(_rootToken, tenantId);
        }

        /// <summary>
        /// Uses the TenantId to retrieve the Environment Code that will be used to
        /// determine the client database, the target for the file insert
        /// </summary>
        /// <param name="rootToken"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<TenantApiResponse> GetShortCodeFromTenantIdAsync(RootAuthToken rootToken, Guid tenantId)
        {
            Ensure.NotNull(rootToken);
            Ensure.NotDefault(tenantId);

            if (!_shortCodeCache.TryGetValue(tenantId, out TenantApiResponse? result))
            {
                result = await GetTenantApiResponse(rootToken, tenantId);
                _shortCodeCache.TryAdd(tenantId, result);
            }
            else
            {
                _logger.LogInformation($"Using cached short code for Tenant {tenantId}");
            }

            return result;
        }

        /// <summary>
        /// Gets a new TenantApiResponse
        /// </summary>
        /// <param name="rootToken"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<TenantApiResponse> GetTenantApiResponse(RootAuthToken rootToken, Guid tenantId)
        {
            Ensure.NotNull(rootToken);
            Ensure.NotDefault(tenantId);

            try
            {
                _logger.LogInformation($"Requesting Tenant short code for Tenant {tenantId}");

                string responseBody = await _jsonClient.GetAsync(
                    $"https://{_settings.TenantHost}/v2/tenant/{tenantId}",
                    rootToken.AccessToken
                );

                return new TenantApiResponse(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred while requesting Tenant short code for Tenant {tenantId}", ex);
                throw;
            }
        }

        /// <summary>
        /// Checks the cache for a client specific auth token. If one does not exist, it is created.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<string> GetToken(Guid tenantId)
        {
            Ensure.NotDefault(tenantId);

            _rootToken ??= await GetRootToken();

            if (!_authTokens.TryGetValue(tenantId, out string? authToken))
            {
                TenantAuthToken tenantApiResponse = await GetTenantToken(tenantId, _rootToken);
                _authTokens[tenantId] = tenantApiResponse.IdToken;
                authToken = tenantApiResponse.IdToken;
            }
            else
            {
                _logger.LogDebug($"Using cached token for Tenant {tenantId}");
            }

            return authToken;
        }

        /// <summary>
        /// Gets a new client specific authentication token for the tenantid provided
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="oAuthToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<TenantAuthToken> GetTenantToken(Guid tenantId, RootAuthToken oAuthToken)
        {
            Ensure.NotDefault(tenantId);
            Ensure.NotNull(oAuthToken);

            try
            {
                _logger.LogInformation($"Requesting Tenant token for Tenant {tenantId}");

                return await _jsonClient.GetAsync<TenantAuthToken>(
                    $"https://{_settings.TenantHost}/rootToken?tenant_id={tenantId}",
                    oAuthToken.AccessToken,
                    false
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"And exception occured while requesting Tenant token for Tenant {tenantId}", ex);
                throw;
            }
        }

        private Dictionary<string, string> ReadSettingsAsDictionary()
        {
            return new Dictionary<string, string>()
            {
                {SettingsNames.ClientIdPropertyName, _settings.ClientId},
                {SettingsNames.ClientSecretPropertyName, _settings.ClientSecret},
                {SettingsNames.ScopePropertyName, _settings.Scope},
                {SettingsNames.GrantTypePropertyName, _settings.GrantType},
            };
        }
    }
}