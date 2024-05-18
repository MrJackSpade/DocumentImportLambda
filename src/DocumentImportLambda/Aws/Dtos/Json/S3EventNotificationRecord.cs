using System.Text.Json.Serialization;

namespace DocumentImportLambda.Aws.Dtos.Json
{
    public class S3EventNotificationRecord
    {
        public S3EventNotificationRecord()
        { }

        public S3EventNotificationRecord(string bucketName, string fileName, Guid tenantId)
        {
            S3 = new S3Data()
            {
                Bucket = new S3Bucket()
                {
                    Name = bucketName
                },
                Object = new S3Object()
                {
                    Key = $"{tenantId}/{fileName}"
                }
            };
        }

        [JsonPropertyName("s3")]
        public S3Data? S3 { get; set; }
    }
}