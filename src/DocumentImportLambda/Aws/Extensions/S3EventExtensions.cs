using DocumentImportLambda.Aws.Dtos.Json;

namespace DocumentImportLambda.Aws.Extensions
{
    public static class S3EventExtensions
    {
        /// <summary>
        /// Validates an s3Event and returns a list of errors
        /// </summary>
        /// <param name="s3Event">The event to validate</param>
        /// <param name="errors">If invalid, errors</param>
        /// <returns>If valid</returns>
        public static bool Validate(this S3Event? s3Event, out List<string> errors)
        {
            errors = [];

            if (s3Event is null)
            {
                errors.Add("s3Event is null");
            }
            else if (s3Event?.Records is null)
            {
                errors.Add("s3Event.Records is null");
            }
            else if (s3Event?.Records.Length == 0)
            {
                errors.Add("s3Event.Records is empty");
            }

            return errors.Count == 0;
        }
    }
}