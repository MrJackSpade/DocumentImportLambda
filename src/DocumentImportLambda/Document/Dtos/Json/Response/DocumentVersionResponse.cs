using System.Text.Json.Serialization;
using DocumentImportLambda.Aws.Dtos.Json;

namespace DocumentImportLambda.Document.Dtos.Json.Response
{
    /// <summary>
    /// Represents an individual document version returned from the DocumentService
    /// </summary>
    public class DocumentVersionResponse(string contentType, string fileName, PresignedLink link, Guid versionId)
    {
        [JsonPropertyName("contentType")]
        public string ContentType { get; } = contentType ?? throw new ArgumentNullException(nameof(contentType));

        [JsonPropertyName("fileName")]
        public string FileName { get; } = fileName ?? throw new ArgumentNullException(nameof(fileName));

        [JsonPropertyName("link")]
        public PresignedLink Link { get; } = link ?? throw new ArgumentNullException(nameof(link));

        [JsonPropertyName("versionId")]
        public Guid VersionId { get; } = versionId;
    }
}