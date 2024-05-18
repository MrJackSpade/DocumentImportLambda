using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class S3Data
    {
        [JsonPropertyName("bucket")]
        public S3Bucket? Bucket { get; set; }

        [JsonPropertyName("object")]
        public S3Object? Object { get; set; }
    }
}