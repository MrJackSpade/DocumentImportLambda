using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using DocumentImportLambda.Aws.Interfaces;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Aws.Services
{
    /// <summary>
    /// This class is used to interface with the S3 bucket containing the uploaded file so that the file contents can be retrieved and posted
    /// to the document service
    /// </summary>
    public class S3BucketService : IReadFileData
    {
        private readonly ILambdaLogger _logger;

        private readonly AmazonS3Client _s3Client = new();

        public S3BucketService(ILambdaLogger logger)
        {
            _logger = Ensure.NotNull(logger);

            logger.LogDebug("Initializing S3 service");
        }

        /// <summary>
        /// Reads the data of the file into a byte array from the S3 bucket
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadFileData(S3Event.S3EventNotificationRecord record)
        {
            Ensure.NotNull(record);

            return await ReadFileData(record.S3.Bucket.Name, record.S3.Object.Key);
        }

        /// <summary>
        /// Reads the data of the file into a byte array from the S3 bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadFileData(string bucketName, string objectKey)
        {
            Ensure.NotNull(bucketName);
            Ensure.NotNull(objectKey);

            _logger.LogInformation($"Requesting S3 object BucketName={bucketName}, Key={objectKey}");
            GetObjectResponse response = await _s3Client.GetObjectAsync(bucketName, objectKey);

            using var memoryStream = new MemoryStream();
            using Stream responseStream = response.ResponseStream;

            _logger.LogInformation($"Reading response stream...");

            // Copy the response stream to the memory stream
            await responseStream.CopyToAsync(memoryStream);

            // Convert the MemoryStream to a byte array
            byte[] fileData = memoryStream.ToArray();

            _logger.LogInformation($"Read {fileData.Length:N0} bytes");

            return fileData;
        }
    }
}