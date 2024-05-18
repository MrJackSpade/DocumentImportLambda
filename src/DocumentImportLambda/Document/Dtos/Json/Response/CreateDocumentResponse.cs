using System.Text.Json.Serialization;

namespace DocumentImportLambda.Document.Dtos.Json.Response
{
    /// <summary>
    /// Represents the top level response to a request to create a new document in the DocumentService
    /// </summary>
    public class CreateDocumentResponse
    {
        [JsonPropertyName("documentId")]
        public Guid DocumentId { get; set; }

        [JsonPropertyName("tenant")]
        public Guid Tenant { get; set; }

        [JsonPropertyName("versions")]
        public DocumentVersionResponse[] Versions { get; set; } = [];
    }
}