using DocumentImportLambda.Authentication.Dtos.Json.Response;
using DocumentImportLambda.Authentication.Interfaces;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    public class DummyRetrieveAuthTokens : IRetrieveAuthTokens
    {
        private readonly TenantApiResponse _tenantApiResponse;

        private readonly string _token;

        public DummyRetrieveAuthTokens(TenantApiResponse? tenantApiResponse = null, string? token = null)
        {
            _tenantApiResponse = tenantApiResponse ?? new TenantApiResponse("{ }");
            _token = token ?? string.Empty;
        }

        public DummyRetrieveAuthTokens(string shortCode, string? token = null)
        {
            _tenantApiResponse = new TenantApiResponse(shortCode, shortCode);
            _token = token ?? string.Empty;
        }

        public Task<TenantApiResponse> GetShortCodeFromTenantIdAsync(Guid tenantId)
        {
            return Task.FromResult(_tenantApiResponse);
        }

        public Task<string> GetToken(Guid tenantId)
        {
            return Task.FromResult(_token);
        }
    }
}