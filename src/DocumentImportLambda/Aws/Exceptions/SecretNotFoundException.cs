namespace DocumentImportLambda.Aws.Exceptions
{
    /// <summary>
    /// An exception thrown when a requested secret with the given name does not exist
    /// in the remote source
    /// </summary>
    /// <param name="secretName"></param>
    public class SecretNotFoundException(string secretName) : Exception($"A secret with the name '{secretName}' was not found")
    {
        public string SecretName { get; private set; } = secretName;
    }
}