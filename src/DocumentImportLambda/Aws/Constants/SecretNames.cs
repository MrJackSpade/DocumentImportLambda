namespace DocumentImportLambda.Aws.Constants
{
    /// <summary>
    /// Class for holding constant values representing the names of secret properties
    /// </summary>
    public static class SecretNames
    {
        public const string ClientId = "oauth_client_id";

        public const string ClientSecret = "oauth_client_secret";

        public const string DatabaseAuthenticationType = "databaseAuthenticationType";

        public const string DatabaseControl = "database_control";

        public const string DatabaseHost = "database_host";

        public const string DatabaseTestBeforeUpload = "databaseTestBeforeUpload";

        public const string DatabaseTrustCertificate = "databaseTrustCertificate";

        public const string DatabaseUserName = "databaseUserName";

        public const string DocServiceRootUrl = "docservice_host";

        public const string IgnoreExtensions = "ignore_extensions";

        public const string OAuthHost = "oauth_host";

        public const string TenantHost = "tenant_host";
    }
}