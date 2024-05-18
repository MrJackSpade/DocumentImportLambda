using DocumentImportLambda.Aws.Dtos.Json;
using System.Diagnostics.CodeAnalysis;

namespace DocumentImportLambda.Aws.Extensions
{
    public static class S3EventNotificationRecordExtensions
    {
        /// <summary>
        /// Validates a record and returns a list of errors
        /// </summary>
        /// <param name="record">The record to validate</param>
        /// <param name="fileName">If valid, the filename</param>
        /// <param name="bucketName">If valid, the bucketname</param>
        /// <param name="errors">If invalid, errors</param>
        /// <returns>If valid</returns>
        public static bool Validate(this S3EventNotificationRecord record,
                                          [NotNullWhen(true)] out string? fileName,
                                          [NotNullWhen(true)] out string? bucketName,
                                          out List<string> errors)
        {
            errors = [];

            if (record is null)
            {
                errors.Add("record is null");
            }
            else if (record?.S3 is null)
            {
                errors.Add("record.S3 is null");
            }
            else
            {
                if (record?.S3?.Object is null)
                {
                    errors.Add("record.S3.Object is null");
                }
                else if (string.IsNullOrWhiteSpace(record?.S3?.Object?.Key))
                {
                    errors.Add("record.S3.Object.Key is null or whitespace");
                }

                if (record?.S3?.Bucket is null)
                {
                    errors.Add("record.S3.Bucket is null");
                }
                else if (string.IsNullOrWhiteSpace(record?.S3?.Bucket?.Name))
                {
                    errors.Add("record.S3.Bucket.Name is null or whitespace");
                }
            }

            fileName = record?.S3?.Object?.Key;
            bucketName = record?.S3?.Bucket?.Name;

            return errors.Count == 0;
        }
    }
}