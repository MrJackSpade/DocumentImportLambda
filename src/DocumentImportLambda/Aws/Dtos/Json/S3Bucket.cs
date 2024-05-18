using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class S3Bucket
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}