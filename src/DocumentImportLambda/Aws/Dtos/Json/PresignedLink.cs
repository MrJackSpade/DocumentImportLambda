using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    /// <summary>
    /// Represents a presigned link returned from the DocumentService, used
    /// to upload the file binary
    /// </summary>
    public class PresignedLink(DateTime expires, string httpVerb, string url)
    {
        [JsonPropertyName("expires")]
        public DateTime Expires { get; } = expires;

        [JsonPropertyName("httpVerb")]
        public string HttpVerb { get; } = httpVerb ?? throw new ArgumentNullException(nameof(httpVerb));

        [JsonPropertyName("url")]
        public string Url { get; } = url ?? throw new ArgumentNullException(nameof(url));
    }
}