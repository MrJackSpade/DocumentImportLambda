using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class RedrivePolicy
    {
        [JsonPropertyName("deadLetterTargetArn")]
        public string DeadLetterTargetArn { get; set; } = string.Empty;
    }
}