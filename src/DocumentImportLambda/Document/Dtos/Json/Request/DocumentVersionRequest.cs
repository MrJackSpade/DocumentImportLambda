using System.Text.Json.Serialization;

namespace DocumentImportLambda.Document.Dtos.Json.Request
{
    /// <summary>
    /// Represents a document version, used when posting a new Document to the DocumentService
    /// </summary>
    public class DocumentVersionRequest
    {
        [JsonPropertyName(nameof(ContentType))]
        public string? ContentType { get; set; }

        [JsonPropertyName(nameof(Creator))]
        public string? Creator { get; set; }

        [JsonPropertyName(nameof(FileName))]
        public string? FileName { get; set; }
    }
}