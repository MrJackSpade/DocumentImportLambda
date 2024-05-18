using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class S3Event
    {
        [JsonPropertyName("Records")]
        public S3EventNotificationRecord[] Records { get; set; } = [];
    }
}