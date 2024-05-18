using DocumentImportLambda.Authentication.Dtos.Json.Response;

namespace DocumentImportLambda.Authentication.Interfaces
{
    /// <summary>
    /// Defines an object capable of both requesting an auth token, as well as
    /// retrieving a shortcode. Theres no logical reason for these to be on
    /// the same interface but then theres no logical reason for them not to.
    /// </summary>
    public interface IRetrieveAuthTokens
    {
        /// <summary>
        /// Returns the tenant shortcode for a given guid
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<TenantApiResponse> GetShortCodeFromTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Gets an authentication token for the given tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<string> GetToken(Guid tenantId);
    }
}