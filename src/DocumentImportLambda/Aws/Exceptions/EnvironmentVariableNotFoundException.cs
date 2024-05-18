namespace DocumentImportLambda.Aws.Exceptions
{
    /// <summary>
    /// An exception thrown when an environment variable with the given name is not found
    /// </summary>
    /// <param name="environmentVariableName"></param>
    public class EnvironmentVariableNotFoundException(string environmentVariableName) : Exception($"An environment variable with the name '{environmentVariableName}' was not found")
    {
        public string EnvironmentVariableName { get; private set; } = environmentVariableName;
    }
}