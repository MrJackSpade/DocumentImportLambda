namespace DocumentImportLambda.Aws.Interfaces
{
    /// <summary>
    /// Defines a method of reading secret values and objects
    /// </summary>
    public interface IReadSecrets
    {
        /// <summary>
        /// Reads a single secret property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        Task<string> Read(string propertyName);
    }
}