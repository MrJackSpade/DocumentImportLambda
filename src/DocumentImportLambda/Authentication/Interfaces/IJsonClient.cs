namespace DocumentImportLambda.Authentication.Interfaces
{
    public interface IJsonClient : IDisposable
    {
        /// <summary>
        /// Posts the object as json to the specified url and checks to ensure the response code is "Created"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="jsonObject"></param>
        /// <param name="bearerToken"></param>
        /// <returns></returns>
        Task<T> CreateAsync<T>(string url, object jsonObject, string bearerToken = "");

        /// <summary>
        /// Gets a string with an optional bearer token for authentication
        /// </summary>
        /// <param name="url"></param>
        /// <param name="bearerToken"></param>
        /// <returns></returns>
        Task<string> GetAsync(string url, string bearerToken = "");

        /// <summary>
        /// Gets a deserialized json object with an optional bearer token for authentication
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="bearerToken"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string url, string bearerToken = "", bool caseSensitive = true);

        /// <summary>
        /// Posts a collection of key-value pairs as a form and then returns the result
        /// as a deserialized json object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        Task<T> PostAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> nameValueCollection);

        /// <summary>
        /// Puts the data at the specified url using the content type provided
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task PutAsync(string url, string contentType, byte[] data);
    }
}