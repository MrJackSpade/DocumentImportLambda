using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class S3Object
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; }
    }
}