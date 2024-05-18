using Amazon.Lambda.Core;
using DocumentImportLambda.Authentication.Constants;
using DocumentImportLambda.Authentication.Interfaces;
using DocumentImportLambda.Utilities;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocumentImportLambda.Authentication.Utilities
{
    /// <summary>
    /// Basically just an HTTP client with Json functionality to reduce boilerplate code.
    /// </summary>
    public class JsonClient : IJsonClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILambdaLogger _logger;

        private readonly bool _ownsHttpClient;

        private bool _disposedValue;

        public JsonClient(HttpClient httpClient, ILambdaLogger logger)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
            _logger = logger;
        }

        public JsonClient(ILambdaLogger logger)
        {
            _httpClient = new HttpClient();
            _ownsHttpClient = true;
            _logger = logger;
        }

        public async Task<T> CreateAsync<T>(string url, object jsonObject, string bearerToken = "")
        {
            Ensure.NotNullOrWhiteSpace(url);
            Ensure.NotNull(jsonObject);

            _logger.LogTrace($"Creating at url: {url}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(jsonObject)
            };

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Headers.Bearer, bearerToken);

            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new HttpRequestException("Unable to create object", null, response.StatusCode);
            }

            return await ReadResponse<T>(response);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<T> GetAsync<T>(string url, string bearerToken = "", bool caseSensitive = true)
        {
            Ensure.NotNullOrWhiteSpace(url);

            string responseBody = await GetAsync(url, bearerToken);

            return JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive = !caseSensitive })!;
        }

        public async Task<string> GetAsync(string url, string bearerToken = "")
        {
            Ensure.NotNullOrWhiteSpace(url);

            _logger.LogTrace($"Getting url: {url}");

            // Create HttpRequestMessage
            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (!string.IsNullOrEmpty(bearerToken))
            {
                // Add the Auth Token to the Request Headers
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Headers.Bearer, bearerToken);
            }

            // Send the request
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            string responseBody = await ReadResponse(response);

            return responseBody;
        }

        public async Task<T> PostAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            Ensure.NotNullOrWhiteSpace(url);
            Ensure.NotNull(nameValueCollection);

            _logger.LogTrace($"Posting url: {url}");

            var content = new FormUrlEncodedContent(nameValueCollection);

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            return await ReadResponse<T>(response);
        }

        public async Task PutAsync(string url, string contentType, byte[] data)
        {
            Ensure.NotNullOrWhiteSpace(url);
            Ensure.NotNullOrWhiteSpace(contentType);
            Ensure.NotNullOrEmpty(data);

            _logger.LogTrace($"Putting url: {url}");

            var toPost = new ByteArrayContent(data);
            toPost.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            HttpResponseMessage response = await _httpClient.PutAsync(url, toPost);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Unable to put data", null, response.StatusCode);
            }

            await ReadResponse(response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_ownsHttpClient)
                    {
                        _httpClient.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        private static async Task<string> ReadResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }

        private static async Task<T> ReadResponse<T>(HttpResponseMessage response, bool caseSensitive = true)
        {
            string responseBody = await ReadResponse(response);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = caseSensitive
            };

            T toReturn = JsonSerializer.Deserialize<T>(responseBody, options)!;

            return toReturn;
        }
    }
}