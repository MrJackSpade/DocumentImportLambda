using System.Text.Json.Serialization;

namespace DocumentImportLambda.Document.Dtos.Json.Request
{
    /// <summary>
    /// Represents a request to upload a document to the DocumentService,
    /// Also contains the file binary used by the AWS request to upload
    /// the content
    /// </summary>
    public class CreateDocumentRequest
    {
        public CreateDocumentRequest(string fileName, string contentType, byte[] content)
        {
            Versions =
            [
                new()
                {
                    ContentType = contentType,
                    FileName = fileName,
                    Creator = Creator
                }
            ];

            Content = content;
        }

        [JsonIgnore]
        public byte[] Content { get; set; }

        [JsonPropertyName(nameof(Creator))]
        public string Creator { get; set; } = "Import Service";

        [JsonPropertyName(nameof(Module))]
        public string Module { get; set; } = "Files";

        [JsonPropertyName(nameof(Versions))]
        public DocumentVersionRequest[] Versions { get; set; } = [];
    }
}