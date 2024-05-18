namespace DocumentImportLambda.Aws.Interfaces
{
    /// <summary>
    /// Defines an interface used to access environment variables
    /// </summary>
    public interface IReadEnvironmentVariables
    {
        /// <summary>
        /// Reads an individual environment variable with a given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string Read(string name);
    }
}