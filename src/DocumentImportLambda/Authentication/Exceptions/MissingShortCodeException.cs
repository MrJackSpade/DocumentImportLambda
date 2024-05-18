namespace DocumentImportLambda.Authentication.Exceptions
{
    /// <summary>
    /// An exception thrown when the tenantId is not found or does not have an associated short code (client name)
    /// </summary>
    /// <param name="tenantId"></param>
    public class MissingShortCodeException(Guid tenantId) : Exception($"SEA Short code not found for TenantId {tenantId}")
    {
        public Guid TenantId { get; } = tenantId;
    }
}