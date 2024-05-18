using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Authentication.Settings
{
    /// <summary>
    /// Represents a set of OAuth settings used to authenticate the client and return the auth token required
    /// by the DocumentService for file uploads
    /// </summary>
    public record OAuthServiceSettings
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string TokenHost { get; }
        public string TenantHost { get; set; }
        public string GrantType { get; }
        public string Scope { get; }

        public OAuthServiceSettings(string? clientId, string? clientSecret, string? tokenHost, string? tenantHost, string grantType = "client_credentials", string scope = "tokex/tenant-root")
        {
            ClientId = Ensure.NotNullOrWhiteSpace(clientId);
            ClientSecret = Ensure.NotNullOrWhiteSpace(clientSecret);
            TokenHost = Ensure.NotNullOrWhiteSpace(tokenHost);
            TenantHost = Ensure.NotNullOrWhiteSpace(tenantHost);
            GrantType = Ensure.NotNullOrWhiteSpace(grantType);
            Scope = Ensure.NotNullOrWhiteSpace(scope);
        }
    }
}