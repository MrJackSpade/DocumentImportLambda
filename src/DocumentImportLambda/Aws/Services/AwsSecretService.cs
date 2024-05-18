using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using DocumentImportLambda.Aws.Exceptions;
using DocumentImportLambda.Aws.Interfaces;
using DocumentImportLambda.Utilities;
using System.Text.Json;

namespace DocumentImportLambda.Aws.Services
{
    /// <summary>
    /// Reads secrets from AWS Secret Manager
    /// </summary>
    /// <param name="region"></param>
    /// <param name="secretName"></param>
    /// <param name="logger"></param>
    public class AwsSecretService(string region, string secretName, ILambdaLogger logger) : IReadSecrets
    {
        private readonly ILambdaLogger _logger = Ensure.NotNull(logger);

        private readonly string _region = Ensure.NotNullOrWhiteSpace(region);

        private readonly string _secretName = Ensure.NotNullOrWhiteSpace(secretName);

        private Dictionary<string, string>? _secrets;

        /// <summary>
        /// Reads a single property from the AWS secret and returns its value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> Read(string key)
        {
            Ensure.NotNullOrWhiteSpace(key);

            _logger.LogDebug($"Reading Key [\"{key}\"]");

            _secrets ??= await LoadSecretAsync();

            return _secrets[key];
        }

        /// <summary>
        /// Loads the secrets from AWS into a dictionary so the properties can be accessed
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SecretNotFoundException"></exception>
        private async Task<Dictionary<string, string>> LoadSecretAsync()
        {
            _logger.LogInformation($"Loading Secrets..");

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(Amazon.RegionEndpoint.GetBySystemName(_region));

            var request = new GetSecretValueRequest() { SecretId = _secretName };

            GetSecretValueResponse? response = await client.GetSecretValueAsync(request);

            if (response?.SecretString is null)
            {
                throw new SecretNotFoundException(_secretName);
            }

            // Parse the secret JSON to retrieve specific values
            Dictionary<string, string> result = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString)!;

            _logger.LogDebug($"Loaded {result.Count} Secrets");

            return result;
        }
    }
}