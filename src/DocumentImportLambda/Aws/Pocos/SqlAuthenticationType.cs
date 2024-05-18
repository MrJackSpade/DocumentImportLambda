namespace DocumentImportLambda.Aws.Pocos
{
    /// <summary>
    /// Enum representing the types of authentication supported for a database connection
    /// </summary>
    public enum SqlAuthenticationType
    {
        /// <summary>
        /// Integrated
        /// </summary>
        Integrated,

        /// <summary>
        /// Sql user pass authentication
        /// </summary>
        Password,

        /// <summary>
        /// Uses the RDS proxy to create a connection
        /// </summary>
        RdsToken
    }
}