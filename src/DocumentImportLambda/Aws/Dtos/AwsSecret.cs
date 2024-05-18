using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Database.Utilities;
using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos
{
    /// <summary>
    /// Represents the secret as stored in AWS
    /// </summary>
    public class AwsSecret
    {
        [JsonPropertyName("databaseAuthenticationType")]
        public SqlAuthenticationType DatabaseAuthenticationType { get; set; }

        [JsonPropertyName("database_control")]
        public string? DatabaseControl { get; set; }

        [JsonPropertyName("database_host")]
        public string? DatabaseHost { get; set; }

        [JsonPropertyName("databaseTestBeforeUpload")]
        public bool DatabaseTestBeforeUpload { get; set; }

        [JsonPropertyName("databaseTrustCertificate")]
        public bool DatabaseTrustCertificate { get; set; }

        [JsonPropertyName("databaseUserName")]
        public string? DatabaseUserName { get; set; }

        [JsonPropertyName("docservice_host")]
        public string? DocServiceHost { get; set; }

        [JsonPropertyName("oauth_client_id")]
        public string? OauthClientId { get; set; }

        [JsonPropertyName("oauth_client_secret")]
        public string? OauthClientSecret { get; set; }

        [JsonPropertyName("oauth_host")]
        public string? OauthHost { get; set; }

        [JsonPropertyName("tenant_host")]
        public string? TenantHost { get; set; }
    }
}