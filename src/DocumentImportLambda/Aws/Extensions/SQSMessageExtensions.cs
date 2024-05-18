using Amazon.Lambda.SQSEvents;

namespace DocumentImportLambda.Aws.Extensions
{
    public static class SQSMessageExtensions
    {
        /// <summary>
        /// Validates a message and returns a list of errors
        /// </summary>
        /// <param name="message">The message to validate</param>
        /// <param name="errors">If invalid, errors</param>
        /// <returns>If valid</returns>
        public static bool Validate(this SQSEvent.SQSMessage message, out List<string> errors)
        {
            errors = [];

            if (message is null)
            {
                errors.Add("message is null");
            }
            else if (message?.Body is null)
            {
                errors.Add("message.Body is null");
            }
            else if (string.IsNullOrWhiteSpace(message?.Body))
            {
                errors.Add("message.Body is null or whitespace");
            }

            return errors.Count == 0;
        }
    }
}