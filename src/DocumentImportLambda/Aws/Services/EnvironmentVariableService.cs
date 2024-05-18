using Amazon.Lambda.Core;
using DocumentImportLambda.Aws.Exceptions;
using DocumentImportLambda.Aws.Interfaces;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Aws.Services
{
    /// <summary>
    /// Interfaces with environment variables to retrieve values.
    /// </summary>
    /// <param name="logger"></param>
    public class EnvironmentVariableService(ILambdaLogger logger) : IReadEnvironmentVariables
    {
        private readonly ILambdaLogger _logger = logger;

        /// <summary>
        /// Read a single environment variable value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="EnvironmentVariableNotFoundException"></exception>
        public string Read(string key)
        {
            Ensure.NotNullOrWhiteSpace(key);

            string value = Environment.GetEnvironmentVariable(key) ?? throw new EnvironmentVariableNotFoundException(key);

            _logger.LogDebug($"Environment Variable [\"{key}\"] = \"{value}\"");

            return value;
        }
    }
}